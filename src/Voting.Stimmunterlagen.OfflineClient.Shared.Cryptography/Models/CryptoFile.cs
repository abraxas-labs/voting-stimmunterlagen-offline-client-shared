// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Models;

internal class CryptoFile
{
    public string Signature { get; set; } = string.Empty;

    public CryptoFileContent Content { get; set; } = new();
}
