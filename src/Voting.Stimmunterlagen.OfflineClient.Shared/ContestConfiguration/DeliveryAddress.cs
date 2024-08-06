// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Newtonsoft.Json;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.ContestConfiguration;

public class DeliveryAddress
{
    [JsonProperty("plz")]
    public string Plz { get; set; } = string.Empty;

    [JsonProperty("municipality")]
    public string Municipality { get; set; } = string.Empty;

    [JsonProperty("street")]
    public string Street { get; set; } = string.Empty;

    [JsonProperty("addressField1")]
    public string AddressField1 { get; set; } = string.Empty;

    [JsonProperty("addressField2")]
    public string? AddressField2 { get; set; }

    [JsonProperty("country")]
    public string Country { get; set; } = string.Empty;
}
