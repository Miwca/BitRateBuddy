﻿using BitRateBuddy.StreamerBot.Messages;
using BitRateBuddy.StreamerBot.Services.Abstractions;
using BitRateBuddy.StreamerBot.Settings;
using Microsoft.Extensions.Options;

namespace BitRateBuddy.StreamerBot.Services
{
    public class WebsocketService(WebsocketClient websocketClient, 
        IOptions<StreamerBotSettings> options) : IStreamerBotService
    {
        public async Task SetStreamLowBitrateAsync(int bitrate, CancellationToken stoppingToken)
        {
            if (!websocketClient.WebsocketIsReady) return;

            var actionId = options.Value.Actions.StreamLowBitrateAction;
            if (string.IsNullOrEmpty(actionId)) return; // Assume not used

            await CallDoActionAsync(actionId, bitrate, stoppingToken);
        }

        public async Task SetStreamOfflineAsync(int bitrate, CancellationToken stoppingToken)
        {
            if (!websocketClient.WebsocketIsReady) return;

            var actionId = options.Value.Actions.StreamOfflineAction;
            if (string.IsNullOrEmpty(actionId)) return; // Assume not used

            await CallDoActionAsync(actionId, bitrate, stoppingToken);
        }

        public async Task SetStreamHealthyAsync(int bitrate, CancellationToken stoppingToken)
        {
            if (!websocketClient.WebsocketIsReady) return;

            var actionId = options.Value.Actions.StreamHealthyAction;
            if (string.IsNullOrEmpty(actionId)) return; // Assume not used

            await CallDoActionAsync(actionId, bitrate, stoppingToken);
        }

        private async Task CallDoActionAsync(string actionId, int bitrate, CancellationToken stoppingToken)
        {
            try
            {
                await websocketClient.SendMessageAsync(new ExecuteActionMessage()
                {
                    Id = Guid.NewGuid().ToString(),
                    Action = actionId
                });
            }
            catch (Exception)
            {
                // Prevent hosted service crash
            }
        }
    }
}
