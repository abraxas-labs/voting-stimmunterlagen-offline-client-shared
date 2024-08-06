// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Models;

internal class CryptoFileContent
{
    public CryptoFileHeader Header { get; set; } = new();

    public CryptoFilePayload Payload { get; set; } = new();
}
