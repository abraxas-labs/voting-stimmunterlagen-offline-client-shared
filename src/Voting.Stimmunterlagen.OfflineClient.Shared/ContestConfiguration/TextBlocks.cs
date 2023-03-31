using Newtonsoft.Json;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.ContestConfiguration;

public class TextBlocks
{
    [JsonProperty("columnQuantity")]
    public object? ColumnQuantity { get; set; }

    [JsonProperty("values")]
    public object? Values { get; set; }
}
