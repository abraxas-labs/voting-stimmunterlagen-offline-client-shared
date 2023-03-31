using System.IO;
using System.Threading.Tasks;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Test;

public abstract class BaseTest
{
    private string AssemblyFolder => Path.GetDirectoryName(GetType().Assembly.Location)!;

    protected Task<byte[]> ReadDummyPdf() => ReadTestFile("dummy.pdf");

    protected Task<byte[]> ReadDummyText() => ReadTestFile("dummy.txt");

    protected Task<byte[]> ReadDummyEncryptedR1Text() => ReadTestFile("dummy-encrypted-r1");

    protected Task<byte[]> ReadInvalidDummyEncryptedR1ManipulatedPayloadText() => ReadTestFile("invalid-dummy-encrypted-r1-manipulated-payload");

    protected Task<byte[]> ReadInvalidDummyEncryptedR1NoReceiversText() => ReadTestFile("invalid-dummy-encrypted-r1-no-receivers");

    protected Task<byte[]> ReadInvalidDummyEncryptedR1NoSenderText() => ReadTestFile("invalid-dummy-encrypted-r1-no-sender");

    protected Task<byte[]> ReadInvalidDummyEncryptedR1WrongHeaderEndText() => ReadTestFile("invalid-dummy-encrypted-r1-wrong-header-end");

    protected Task<byte[]> ReadInvalidDummyEncryptedR1WrongHeaderStartText() => ReadTestFile("invalid-dummy-encrypted-r1-wrong-header-start");

    protected Task<byte[]> ReadInvalidDummyEncryptedR1WrongSignatureText() => ReadTestFile("invalid-dummy-encrypted-r1-wrong-signature");

    protected async Task<byte[]> ReadTestFile(string fileName)
    {
        return await File.ReadAllBytesAsync(Path.Join(AssemblyFolder, $"TestFiles/{fileName}"));
    }
}
