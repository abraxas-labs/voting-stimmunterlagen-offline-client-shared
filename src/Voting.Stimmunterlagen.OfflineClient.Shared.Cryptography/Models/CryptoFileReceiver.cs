// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Models;

internal class CryptoFileReceiver
{
    public string Id { get; set; } = string.Empty;

    public string Salt { get; set; } = string.Empty;

    public string Payload { get; set; } = string.Empty;
}
