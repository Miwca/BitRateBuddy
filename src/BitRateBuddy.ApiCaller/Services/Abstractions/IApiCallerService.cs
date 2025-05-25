namespace BitRateBuddy.ApiCaller.Services.Abstractions
{
    public interface IApiCallerService
    {
        Task<string> GetAsync(string clientName, string path, CancellationToken stoppingToken);

        Task<string> PostAsync(string clientName, string path, HttpContent content,
            CancellationToken stoppingToken);
    }
}
