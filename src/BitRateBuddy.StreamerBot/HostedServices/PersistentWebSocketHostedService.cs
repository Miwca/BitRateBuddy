using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using BitRateBuddy.StreamerBot.Messages;
using BitRateBuddy.StreamerBot.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BitRateBuddy.StreamerBot.HostedServices;

public class PersistentWebSocketHostedService(
    ILogger<PersistentWebSocketHostedService> logger,
    IOptions<StreamerBotSettings> options,
    WebsocketClient websocketClient)
    : BackgroundService
{
    private readonly Uri _serverUri = new(options.Value.WebsocketUrl);
    
    private readonly TimeSpan _reconnectDelay = TimeSpan.FromSeconds(5);

    private ClientWebSocket? _webSocket;

    public event Action<string>? MessageReceived;

    public async Task SendAsync(string message)
    {
        await websocketClient.SendQueue.Writer.WriteAsync(message);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _webSocket = new ClientWebSocket();
                await _webSocket.ConnectAsync(_serverUri, stoppingToken);
                logger.LogInformation("WebSocket connected.");

                var receiveTask = ReceiveLoopAsync(_webSocket, stoppingToken);
                var sendTask = SendLoopAsync(_webSocket, stoppingToken);

                await Task.WhenAny(receiveTask, sendTask); // End early on error
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "WebSocket error.");
                websocketClient.WebsocketIsReady = false;
            }

            try
            {
                if (_webSocket?.State == WebSocketState.Open || _webSocket?.State == WebSocketState.CloseReceived)
                {
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Reconnecting", stoppingToken);
                }
            }
            catch { /* ignore */ }

            _webSocket?.Dispose();
            logger.LogWarning("WebSocket disconnected. Reconnecting in {Delay}s...", _reconnectDelay.TotalSeconds);
            await Task.Delay(_reconnectDelay, stoppingToken);
        }
    }

    private async Task SendLoopAsync(ClientWebSocket ws, CancellationToken token)
    {
        while (!token.IsCancellationRequested && ws.State == WebSocketState.Open)
        {
            while (websocketClient.SendQueue.Reader.TryRead(out var msg))
            {
                var buffer = Encoding.UTF8.GetBytes(msg);
                await ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, token);
                logger.LogInformation("SENT: {Message}", msg);
            }

            await Task.Delay(100, token); // prevent tight loop
        }
    }

    private async Task ReceiveLoopAsync(ClientWebSocket ws, CancellationToken token)
    {
        var buffer = new byte[4096];

        while (!token.IsCancellationRequested && ws.State == WebSocketState.Open)
        {
            var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), token);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                logger.LogInformation("Received close from server.");
                break;
            }

            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            logger.LogInformation("RECEIVED: {Message}", message);

            MessageReceived?.Invoke(message);
            if (!message.Contains("\"authentication\"", StringComparison.OrdinalIgnoreCase)) continue;

            var authMessage = JsonConvert.DeserializeObject<AuthenticationParentMessage>(message);
            if (authMessage?.Authentication == null) continue;

            await AuthenticateConnection(authMessage);
            logger.LogInformation("AUTHENTICATION SENT");

            // wait for proper auth or websocket server in Streamer.Bot crashes.
            await Task.Delay(5000, token); 
            websocketClient.WebsocketIsReady = true;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping WebSocket client...");

        if (_webSocket?.State == WebSocketState.Open || _webSocket?.State == WebSocketState.CloseReceived)
        {
            try
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Shutdown", cancellationToken);
            }
            catch
            {
                // Ignore exceptions while closing the connection...
            }
        }

        _webSocket?.Dispose();
        await base.StopAsync(cancellationToken);
    }

    private async Task AuthenticateConnection(AuthenticationParentMessage authMessage)
    {
        var password = options.Value.Password;

        var auth = authMessage.Authentication;
        var salt = password + auth.Salt;
        var base64Salt = Convert.ToBase64String(ToSha256(salt));
        var base64Auth = Convert.ToBase64String(ToSha256(base64Salt + auth.Challenge));

        var authPayload = JsonConvert.SerializeObject(new
        {
            id = "BitRateBuddy",
            request = "Authenticate",
            authentication = base64Auth
        });

        await SendAsync(authPayload);
    }

    private static byte[] ToSha256(string str)
    {
        using var sha256 = SHA256.Create();
        var hashArray = sha256.ComputeHash(Encoding.UTF8.GetBytes(str));
        return hashArray;
    }
}