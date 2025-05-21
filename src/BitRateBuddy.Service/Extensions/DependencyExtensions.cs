using BitRateBuddy.NginxStats.HostedServices;
using BitRateBuddy.NginxStats.Settings;
using BitRateBuddy.StreamerBot;
using BitRateBuddy.StreamerBot.HostedServices;
using BitRateBuddy.StreamerBot.Services;
using BitRateBuddy.StreamerBot.Settings;

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

            // Hosted Services
            services.AddHostedService<StreamWatcherHostedService>();
            services.AddHostedService<PersistentWebSocketHostedService>();

            // Services
            services.AddSingleton<WebsocketService>();

            // Clients
            services.AddSingleton<WebsocketClient>();

            services.AddHttpClient(
                "nginx-stats",
                client =>
                {
                    // Add a user-agent default request header.
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("bitratebuddy");
                }
            );

            return services;
        }
    }
}
