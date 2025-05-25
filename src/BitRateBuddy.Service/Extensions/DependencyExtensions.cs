using BitRateBuddy.ApiCaller;
using BitRateBuddy.ApiCaller.Services;
using BitRateBuddy.ApiCaller.Services.Abstractions;
using BitRateBuddy.NginxStats.HostedServices;
using BitRateBuddy.NginxStats.Services;
using BitRateBuddy.NginxStats.Services.Abstractions;
using BitRateBuddy.NginxStats.Settings;
using BitRateBuddy.StreamerBot.Services;
using BitRateBuddy.StreamerBot.Services.Abstractions;
using BitRateBuddy.StreamerBot.Settings;
using Microsoft.Extensions.Options;

namespace BitRateBuddy.Service.Extensions
{
    public static class DependencyExtensions
    {
        public static IServiceCollection RegisterDependencies(this IServiceCollection services,
            IConfiguration configuration)
        {
            // Config
            services.Configure<StreamSettings>(configuration.GetSection("StreamSettings"));
            services.Configure<StreamerBotSettings>(configuration.GetSection("StreamerBotSettings"));

            // Api caller
            services.AddSingleton<IApiCallerService, ApiCallerService>();
            services.AddSingleton<ApiCallerClient>();

            // NGINX-RTMP
            services.AddHostedService<StreamWatcherHostedService>();
            services.AddSingleton<IStreamWatcherService, StreamWatcherService>();

            // StreamerBot
            services.AddSingleton<IStreamerBotService, HttpService>();

            // Currently a bug in Streamer.Bot 0.2.8 that won't be fixed until 1.0
            // where the socket state ends up in an invalid state. WebSocketService
            // should however still be working, so leaving it for the future.
            //services.AddHostedService<PersistentWebSocketHostedService>();
            //services.AddSingleton<IStreamerBotService, WebsocketService>();
            //services.AddSingleton<WebsocketClient>();

            // HTTP Clients
            services.AddHttpClient(
                "nginx-stats",
                (sp, client) =>
                {
                    var options = sp.GetRequiredService<IOptions<StreamSettings>>();
                    client.BaseAddress = new Uri(options.Value.BaseUrl);

                    // Add a user-agent default request header.
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("bitratebuddy");
                }
            );

            services.AddHttpClient(
                "streamerbot",
                (sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<StreamerBotSettings>>();
                client.BaseAddress = new Uri(options.Value.Http.BaseUrl);

                    // Add a user-agent default request header.
                client.DefaultRequestHeaders.UserAgent.ParseAdd("bitratebuddy");
                }
            );

            return services;
        }
    }
}
