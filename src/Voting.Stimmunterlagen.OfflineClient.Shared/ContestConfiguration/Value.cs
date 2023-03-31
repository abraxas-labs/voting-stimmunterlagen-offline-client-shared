using Newtonsoft.Json;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.ContestConfiguration;

public class Value
{
    [JsonProperty("title")]
    public string? Title { get; set; }

    [JsonProperty("text")]
    public string? Text { get; set; }
}
