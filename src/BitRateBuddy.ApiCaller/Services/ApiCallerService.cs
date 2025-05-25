using BitRateBuddy.ApiCaller.Exceptions;
using BitRateBuddy.ApiCaller.Services.Abstractions;
using Microsoft.Extensions.Logging;

namespace BitRateBuddy.ApiCaller.Services
{
    public class ApiCallerService(ILogger<ApiCallerService> logger, ApiCallerClient apiClient) : IApiCallerService
    {
        public async Task<string> GetAsync(string clientName, string path, CancellationToken stoppingToken)
        {
            try
            {
                var result = await apiClient.GetAsync(clientName, path, stoppingToken);
                return result;
            }
            catch (ApiCallerException ace)
            {
                logger.LogError(ace, "Server did not respond with an OK result");
                throw;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Unhandled exception occurred making API call");
                throw;
            }
        }

        public async Task<string> PostAsync(string clientName, string path, HttpContent content,
            CancellationToken stoppingToken)
        {
            try
            {
                var result = await apiClient.PostAsync(clientName, path, content, stoppingToken);
                return result;
            }
            catch (ApiCallerException ace)
            {
                logger.LogError(ace, "Server did not respond with an OK result");
                throw;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Unhandled exception occurred making API call");
                throw;
            }
        }
    }
}
