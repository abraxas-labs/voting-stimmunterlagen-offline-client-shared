// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.IO;
using System.Threading.Tasks;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Test;

public abstract class BaseTest
{
    private string AssemblyFolder => Path.GetDirectoryName(GetType().Assembly.Location)!;

    protected Task<byte[]> ReadDummyPdf() => ReadTestFile("dummy.pdf");

    protected Task<byte[]> ReadDummyText() => ReadTestFile("dummy.txt.bin");

    protected Task<byte[]> ReadDummyEncryptedR1Text() => ReadTestFile("dummy-encrypted-r1.bin");

    protected Task<byte[]> ReadInvalidDummyEncryptedR1ManipulatedPayloadText() => ReadTestFile("invalid-dummy-encrypted-r1-manipulated-payload.bin");

    protected Task<byte[]> ReadInvalidDummyEncryptedR1ManipulatedSignatureText() => ReadTestFile("invalid-dummy-encrypted-r1-manipulated-signature.bin");

    protected Task<byte[]> ReadInvalidDummyEncryptedR1NoVersionText() => ReadTestFile("invalid-dummy-encrypted-r1-no-version.bin");

    protected Task<byte[]> ReadInvalidDummyEncryptedR1InvalidVersionText() => ReadTestFile("invalid-dummy-encrypted-r1-invalid-version.bin");

    protected Task<byte[]> ReadInvalidDummyEncryptedR1NoReceiversText() => ReadTestFile("invalid-dummy-encrypted-r1-no-receivers.bin");

    protected Task<byte[]> ReadInvalidDummyEncryptedR1NoSenderText() => ReadTestFile("invalid-dummy-encrypted-r1-no-sender.bin");

    protected Task<byte[]> ReadInvalidDummyEncryptedR1InvalidSenderText() => ReadTestFile("invalid-dummy-encrypted-r1-invalid-sender.bin");

    protected Task<byte[]> ReadInvalidDummyEncryptedR1InvalidReceiverText() => ReadTestFile("invalid-dummy-encrypted-r1-invalid-receiver.bin");

    protected Task<byte[]> ReadInvalidDummyEncryptedR1ReceiverLeadingSenderText() => ReadTestFile("invalid-dummy-encrypted-r1-sender-leading-receiver.bin");

    protected Task<byte[]> ReadInvalidDummyEncryptedR1WrongHeaderEndText() => ReadTestFile("invalid-dummy-encrypted-r1-wrong-header-end.bin");

    protected Task<byte[]> ReadInvalidDummyEncryptedR1WrongHeaderStartText() => ReadTestFile("invalid-dummy-encrypted-r1-wrong-header-start.bin");

    protected async Task<byte[]> ReadTestFile(string fileName)
    {
        return await File.ReadAllBytesAsync(Path.Join(AssemblyFolder, $"TestFiles/{fileName}"));
    }
}
