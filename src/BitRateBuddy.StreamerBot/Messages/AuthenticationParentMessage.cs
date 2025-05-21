using Newtonsoft.Json;

namespace BitRateBuddy.StreamerBot.Messages;

internal class AuthenticationParentMessage
{
    [JsonProperty("authentication")]
    public AuthenticationMessage Authentication { get; set; } = new();
}