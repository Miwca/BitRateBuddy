using BitRateBuddy.StreamerBot.Messages;
using BitRateBuddy.StreamerBot.Settings;
using Microsoft.Extensions.Options;

namespace BitRateBuddy.StreamerBot.Services
{
    public class WebsocketService(WebsocketClient websocketClient, 
        IOptions<StreamerBotSettings> options)
    {
        public async Task SetStreamLowBitrateAsync()
        {
            if (!websocketClient.WebsocketIsReady) return;

            var lowBitrateAction = options.Value.Actions.StreamLowBitrateAction;
            if (string.IsNullOrEmpty(lowBitrateAction)) return; // Assume not used

            await websocketClient.SendMessageAsync(new ExecuteActionMessage()
            {
                Id = Guid.NewGuid().ToString(),
                Action = lowBitrateAction
            });
        }

        public async Task SetStreamOfflineAsync()
        {
            if (!websocketClient.WebsocketIsReady) return;

            var lowBitrateAction = options.Value.Actions.StreamOfflineAction;
            if (string.IsNullOrEmpty(lowBitrateAction)) return; // Assume not used

            await websocketClient.SendMessageAsync(new ExecuteActionMessage()
            {
                Id = Guid.NewGuid().ToString(),
                Action = lowBitrateAction
            });
        }

        public async Task SetStreamHealthyAsync()
        {
            if (!websocketClient.WebsocketIsReady) return;

            var lowBitrateAction = options.Value.Actions.StreamHealthyAction;
            if (string.IsNullOrEmpty(lowBitrateAction)) return; // Assume not used

            await websocketClient.SendMessageAsync(new ExecuteActionMessage()
            {
                Id = Guid.NewGuid().ToString(),
                Action = lowBitrateAction
            });
        }
    }
}
