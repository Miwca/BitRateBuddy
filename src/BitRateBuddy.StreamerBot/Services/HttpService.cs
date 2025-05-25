using BitRateBuddy.ApiCaller.Services.Abstractions;
using BitRateBuddy.StreamerBot.Services.Abstractions;
using BitRateBuddy.StreamerBot.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BitRateBuddy.StreamerBot.Services
{
    public class HttpService(IApiCallerService apiCallerService, IOptions<StreamerBotSettings> options) : IStreamerBotService
    {
        private const string ClientName = "streamerbot";

        public async Task SetStreamLowBitrateAsync(int bitrate, CancellationToken stoppingToken)
        {
            var actionId = options.Value.Actions.StreamLowBitrateAction;
            if (string.IsNullOrEmpty(actionId)) return; // Assume not used

            await CallDoActionAsync(actionId, bitrate, stoppingToken);
        }

        public async Task SetStreamOfflineAsync(int bitrate, CancellationToken stoppingToken)
        {
            var actionId = options.Value.Actions.StreamOfflineAction;
            if (string.IsNullOrEmpty(actionId)) return; // Assume not used

            await CallDoActionAsync(actionId, bitrate, stoppingToken);
        }

        public async Task SetStreamHealthyAsync(int bitrate, CancellationToken stoppingToken)
        {
            var actionId = options.Value.Actions.StreamHealthyAction;
            if (string.IsNullOrEmpty(actionId)) return; // Assume not used

            await CallDoActionAsync(actionId, bitrate, stoppingToken);
        }

        private async Task CallDoActionAsync(string actionId, int bitrate, CancellationToken stoppingToken)
        {
            try
            {
                // Returns a 204 No Content - So no need reading the response body.
                await apiCallerService.PostAsync(ClientName, "/DoAction", new StringContent(JsonConvert.SerializeObject(new
                {
                    action = new
                    {
                        id = actionId,
                        name = "<name>" // Is this one needed?
                    },
                    args = new
                    {
                        bitrate = bitrate.ToString()
                    }
                })), stoppingToken);
            }
            catch (Exception)
            {
                // Prevent hosted service crash
            }
        }
    }
}
