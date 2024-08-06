// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.ContestConfiguration;

public class Configuration
{
    [JsonProperty("polldate")]
    public string Polldate { get; set; } = string.Empty;

    [JsonProperty("certificates")]
    public List<string> Certificates { get; set; } = new();

    [JsonProperty("printings")]
    public List<Printing> Printings { get; set; } = new();
}
