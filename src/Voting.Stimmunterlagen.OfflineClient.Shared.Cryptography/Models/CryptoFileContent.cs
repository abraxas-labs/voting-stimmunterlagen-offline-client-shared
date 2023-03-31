namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Models;

internal class CryptoFileContent
{
    public CryptoFileHeader Header { get; set; } = new();

    public CryptoFilePayload Payload { get; set; } = new();
}
