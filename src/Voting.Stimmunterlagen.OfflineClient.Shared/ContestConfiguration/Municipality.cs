using Newtonsoft.Json;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.ContestConfiguration;

public class Municipality
{
    [JsonProperty("bfs")]
    public string Bfs { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("logo")]
    public string? Logo { get; set; }

    [JsonProperty("template")]
    public object? Template { get; set; }

    [JsonProperty("etemplate")]
    public string? Etemplate { get; set; }

    [JsonProperty("pollOpening")]
    public string PollOpening { get; set; } = string.Empty;

    [JsonProperty("pollClosing")]
    public string PollClosing { get; set; } = string.Empty;

    [JsonProperty("deliveryType")]
    public string DeliveryType { get; set; } = string.Empty;

    [JsonProperty("forwardDeliveryType")]
    public string ForwardDeliveryType { get; set; } = string.Empty;

    [JsonProperty("returnDeliveryType")]
    public string ReturnDeliveryType { get; set; } = string.Empty;

    [JsonProperty("returnDeliveryAddress")]
    public DeliveryAddress ReturnDeliveryAddress { get; set; } = new();

    [JsonProperty("textBlocks")]
    public TextBlocks? TextBlocks { get; set; }

    [JsonProperty("eTextBlocks")]
    public ETextBlocks? ETextBlocks { get; set; }

    [JsonProperty("vcteVotingFingerprint")]
    public string? VcteVotingFingerprint { get; set; }
}
