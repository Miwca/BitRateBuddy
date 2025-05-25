namespace BitRateBuddy.StreamerBot.Services.Abstractions
{
    public interface IStreamerBotService
    {
        Task SetStreamLowBitrateAsync(int bitrate, CancellationToken stoppingToken);
        Task SetStreamOfflineAsync(int bitrate, CancellationToken stoppingToken);
        Task SetStreamHealthyAsync(int bitrate, CancellationToken stoppingToken);
    }
}
