namespace BitRateBuddy.StreamerBot.Settings
{
    public class StreamerBotSettings
    {
        public string WebsocketUrl { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public StreamerBotActionsSettings Actions { get; set; } = new();
    }

    public class StreamerBotActionsSettings
    {
        public string StreamLowBitrateAction { get; set; } = string.Empty;
        public string StreamOfflineAction { get; set; } = string.Empty;
        public string StreamHealthyAction { get; set; } = string.Empty;
    }
}
