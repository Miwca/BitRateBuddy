namespace BitRateBuddy.NginxStats.Settings
{
    public class StreamSettings
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string StreamKey { get; set; } = string.Empty;
        public int LowBitrateThreshold { get; set; }
    }
}
