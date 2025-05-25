using BitRateBuddy.ApiCaller.Exceptions;

namespace BitRateBuddy.ApiCaller
{
    public class ApiCallerClient(IHttpClientFactory httpClientFactory)
    {
        internal async Task<string> GetAsync(string clientName, string path, CancellationToken stoppingToken)
        {
            var httpClient = httpClientFactory.CreateClient(clientName);
            var response = await httpClient.GetAsync(path, stoppingToken);
            if (response is not { IsSuccessStatusCode: true })
            {
                throw new ApiCallerException(response.StatusCode, "Server did not return an OK response");
            }

            var result = await response.Content.ReadAsStringAsync(stoppingToken);
            return result;
        }

        internal async Task<string> PostAsync(string clientName, string path, HttpContent 
            content, CancellationToken stoppingToken)
        {
            var httpClient = httpClientFactory.CreateClient(clientName);
            var response = await httpClient.PostAsync(path, content, stoppingToken);
            if (response is not { IsSuccessStatusCode: true })
            {
                throw new ApiCallerException(response.StatusCode, "Server did not return an OK response");
            }

            var result = await response.Content.ReadAsStringAsync(stoppingToken);
            return result;
        }
    }
}
