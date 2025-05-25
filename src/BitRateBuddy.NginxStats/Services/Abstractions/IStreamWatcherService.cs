namespace BitRateBuddy.NginxStats.Services.Abstractions
{
    public interface IStreamWatcherService
    {
        Task ParseResultAsync(int bitrate, CancellationToken stoppingToken);
    }
}
