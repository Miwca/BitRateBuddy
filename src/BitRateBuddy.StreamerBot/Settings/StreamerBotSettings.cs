namespace BitRateBuddy.StreamerBot.Settings
{
    public class StreamerBotSettings
    {
        public StreamerBotWebsocketSettings Websocket { get; set; } = new();
        public StreamerBotUrlSettings Http { get; set; } = new();
        public StreamerBotActionsSettings Actions { get; set; } = new();
    }

    public class StreamerBotWebsocketSettings
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class StreamerBotUrlSettings
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class StreamerBotActionsSettings
    {
        public string StreamLowBitrateAction { get; set; } = string.Empty;
        public string StreamOfflineAction { get; set; } = string.Empty;
        public string StreamHealthyAction { get; set; } = string.Empty;
    }
}
