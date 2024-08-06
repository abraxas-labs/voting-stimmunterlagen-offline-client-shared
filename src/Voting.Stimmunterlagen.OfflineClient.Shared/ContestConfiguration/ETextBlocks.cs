// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.ContestConfiguration;

public class ETextBlocks
{
    [JsonProperty("columnQuantity")]
    public string? ColumnQuantity { get; set; }

    [JsonProperty("values")]
    public List<Value>? Values { get; set; }
}
