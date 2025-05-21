using System.Threading.Channels;
using BitRateBuddy.StreamerBot.Messages.Abstractions;
using Newtonsoft.Json;

namespace BitRateBuddy.StreamerBot
{
    public class WebsocketClient
    {
        internal readonly Channel<string> SendQueue = Channel.CreateUnbounded<string>();
        internal bool WebsocketIsReady = false;

        public async Task SendMessageAsync(IMessage message)
        {
            await SendQueue.Writer.WriteAsync(JsonConvert.SerializeObject(message));
        }
    }
}
