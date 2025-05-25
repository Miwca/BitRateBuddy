using BitRateBuddy.NginxStats.Enums;
using BitRateBuddy.NginxStats.Services.Abstractions;
using BitRateBuddy.NginxStats.Settings;
using BitRateBuddy.StreamerBot.Services.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BitRateBuddy.NginxStats.Services
{
    public class StreamWatcherService(ILogger<StreamWatcherService> logger, 
        IStreamerBotService streamerBotService, IOptions<StreamSettings> options) : IStreamWatcherService
    {
        private StreamStateEnum _streamState;
        private int _bitrate;

        public async Task ParseResultAsync(int bitrate, CancellationToken stoppingToken)
        {
            var unhealthyThreshold = options.Value.LowBitrateThreshold;
            StreamStateEnum newStreamState;
            if (bitrate == 0)
            {
                logger.LogDebug("Stream is offline or not found.");
                newStreamState = StreamStateEnum.Offline;
            }
            else if (bitrate <= unhealthyThreshold)
            {
                logger.LogDebug("Low bitrate detected.");
                newStreamState = StreamStateEnum.Unhealthy;
            }
            else
            {
                logger.LogDebug("Stream is above healthy threshold");
                newStreamState = StreamStateEnum.Healthy;
            }

            await CallStreamerBot(newStreamState, bitrate, stoppingToken);
        }

        private async Task CallStreamerBot(StreamStateEnum newStreamState, int newBitrate, 
            CancellationToken stoppingToken)
        {
            // Prevents calling streamerBot if unnecessary
            if (newStreamState == _streamState && newBitrate == _bitrate) return;

            logger.LogInformation(
                $"StreamState or Bitrate changed. StreamState: {newStreamState}, Bitrate: {newBitrate}");

            switch (newStreamState)
            {
                case StreamStateEnum.Unknown:
                case StreamStateEnum.Healthy:
                    await streamerBotService.SetStreamHealthyAsync(newBitrate, stoppingToken);
                    break;
                case StreamStateEnum.Unhealthy:
                    await streamerBotService.SetStreamLowBitrateAsync(newBitrate, stoppingToken);
                    break;
                case StreamStateEnum.Offline:
                    await streamerBotService.SetStreamOfflineAsync(newBitrate, stoppingToken);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newStreamState), newStreamState, null);
            }

            _streamState = newStreamState;
            _bitrate = newBitrate;
        }
    }
}
