// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.ContestConfiguration;

public class Printing
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("municipalities")]
    public List<Municipality> Municipalities { get; set; } = new();
}
