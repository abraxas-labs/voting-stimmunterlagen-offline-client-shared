namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Models;

internal class CryptoFile
{
    public string Signature { get; set; } = string.Empty;

    public CryptoFileContent Content { get; set; } = new();
}
