using BitRateBuddy.StreamerBot.Messages.Abstractions;
using Newtonsoft.Json;

namespace BitRateBuddy.StreamerBot.Messages;

internal class ExecuteActionMessage : IMessage
{
    [JsonProperty("request")]
    public string Request => "DoAction";

    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("action")]
    public string Action { get; set; } = string.Empty;
}