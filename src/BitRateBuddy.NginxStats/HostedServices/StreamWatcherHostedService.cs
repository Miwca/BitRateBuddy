using System.Xml.Linq;
using BitRateBuddy.NginxStats.Settings;
using BitRateBuddy.StreamerBot.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Formatting = System.Xml.Formatting;

namespace BitRateBuddy.NginxStats.HostedServices
{
    public class StreamWatcherHostedService(ILogger<StreamWatcherHostedService> logger,
        IHttpClientFactory httpClientFactory, IOptions<StreamSettings> streamSettings,
        WebsocketService websocketService)
        : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var statUrl = streamSettings.Value.StatUrl;
            var streamKey = streamSettings.Value.StreamKey;

            logger.LogInformation("RTMP Stat Monitor started.");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var httpClient = httpClientFactory.CreateClient("nginx-stats");

                    var xml = await httpClient.GetStringAsync(statUrl, stoppingToken);
                    var doc = XDocument.Parse(xml);
                    var stream = doc.Descendants("stream")
                        .FirstOrDefault(s => s.Element("name")?.Value == streamKey);

                    if (stream == null)
                    {
                        logger.LogWarning("Stream is offline or not found.");
                        await websocketService.SetStreamOfflineAsync();
                    }
                    else
                    {
                        var bwInStr = stream.Element("bw_in")?.Value ?? "0"; // Divide by 100
                        if (int.TryParse(bwInStr, out var bitrateBytes))
                        {
                            var bitrateKiloBytes = bitrateBytes / 1000;
                            logger.LogInformation($"Stream '{streamKey}' is live. Bitrate in: {bitrateKiloBytes} kbps");

                            if (bitrateKiloBytes == 0)
                            {
                                logger.LogWarning("Stream is offline or not sending");
                                await websocketService.SetStreamOfflineAsync();
                            }
                            switch (bitrateKiloBytes)
                            {
                                case <= 500:
                                    logger.LogWarning("Low bitrate detected.");
                                    await websocketService.SetStreamLowBitrateAsync();
                                    break;
                                case > 500:
                                    logger.LogInformation("Stream is above healthy threshold");
                                    await websocketService.SetStreamHealthyAsync();
                                    break;
                            }
                        }

                        var json = JsonConvert.SerializeXNode(stream, (Newtonsoft.Json.Formatting)Formatting.Indented);
                        logger.LogDebug("Stream JSON:\n" + json);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to fetch or parse RTMP stats.");
                }

                await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken); // Poll every 10s
            }
        }
    }
}
