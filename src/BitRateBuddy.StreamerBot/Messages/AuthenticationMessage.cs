using Newtonsoft.Json;

namespace BitRateBuddy.StreamerBot.Messages
{
    internal class AuthenticationMessage
    {
        [JsonProperty("salt")]
        public string Salt { get; set; } = string.Empty;

        [JsonProperty("challenge")]
        public string Challenge { get; set; } = string.Empty;
    }
}
