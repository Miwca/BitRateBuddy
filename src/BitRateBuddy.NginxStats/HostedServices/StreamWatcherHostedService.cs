using System.Xml.Linq;
using BitRateBuddy.NginxStats.Services.Abstractions;
using BitRateBuddy.NginxStats.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BitRateBuddy.NginxStats.HostedServices
{
    public class StreamWatcherHostedService(ILogger<StreamWatcherHostedService> logger,
        IHttpClientFactory httpClientFactory, IOptions<StreamSettings> streamSettings,
        IStreamWatcherService streamWatcherService)
        : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var streamKey = streamSettings.Value.StreamKey;

            logger.LogInformation("RTMP Stat Monitor started.");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var httpClient = httpClientFactory.CreateClient("nginx-stats");

                    var xml = await httpClient.GetStringAsync("/stat", stoppingToken);
                    var doc = XDocument.Parse(xml);
                    var stream = doc.Descendants("stream")
                        .FirstOrDefault(s => s.Element("name")?.Value == streamKey);

                    if (stream == null)
                    {
                        await streamWatcherService.ParseResultAsync(0, stoppingToken);
                    }
                    else
                    {
                        var bwInStr = stream.Element("bw_in")?.Value ?? "0";
                        if (!int.TryParse(bwInStr, out var bitrateBytes)) continue;
                        
                        var bitrateKiloBytes = bitrateBytes / 1000;
                        logger.LogDebug($"Stream '{streamKey}' is live. Bitrate in: {bitrateKiloBytes} kbps");

                        await streamWatcherService.ParseResultAsync(bitrateKiloBytes, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to fetch or parse RTMP stats.");
                }

                await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken); // Poll every 3s
            }
        }
    }
}
