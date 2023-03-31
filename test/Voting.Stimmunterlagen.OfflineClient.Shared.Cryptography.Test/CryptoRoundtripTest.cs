using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Decryption;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Encryption;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Exceptions;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Mocks.Storage;
using Xunit;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Test;

public class CryptoRoundtripTest : BaseTest
{
    private readonly FileEncryptor _encryptor = new(new Mock<ILogger<FileEncryptor>>().Object);
    private readonly FileDecryptor _decryptor = new(new Mock<ILogger<FileDecryptor>>().Object);

    [Fact]
    public async Task ShouldWorkWithPlainText()
    {
        var plaintextBytes = await ReadDummyText();

        var plaintext = Encoding.UTF8.GetString(plaintextBytes);

        var decryptedPlaintextBytes = Roundtrip(
            plaintextBytes,
            WindowsCertificateStoreMock.SenderPrivateCertificate,
            new() { WindowsCertificateStoreMock.Receiver1PublicCertificate },
            WindowsCertificateStoreMock.SenderPublicCertificate,
            WindowsCertificateStoreMock.Receiver1PrivateCertificate);

        var decryptedPlaintext = Encoding.UTF8.GetString(decryptedPlaintextBytes);
        decryptedPlaintext.Should().Be(plaintext);
    }

    [Fact]
    public async Task ShouldWorkWithBinaryFile()
    {
        var plaintextBytes = await ReadDummyPdf();

        var decryptedPlaintextBytes = Roundtrip(
            plaintextBytes,
            WindowsCertificateStoreMock.SenderPrivateCertificate,
            new() { WindowsCertificateStoreMock.Receiver1PublicCertificate },
            WindowsCertificateStoreMock.SenderPublicCertificate,
            WindowsCertificateStoreMock.Receiver1PrivateCertificate);

        plaintextBytes.SequenceEqual(decryptedPlaintextBytes).Should().BeTrue();
    }

    [Fact]
    public async Task ShouldWorkWithMultipleReceivers()
    {
        var plaintextBytes = await ReadDummyPdf();

        var decryptedPlaintextBytesList = Roundtrip(
            plaintextBytes,
            WindowsCertificateStoreMock.SenderPrivateCertificate,
            new List<X509Certificate2>
            {
                WindowsCertificateStoreMock.Receiver1PublicCertificate,
                WindowsCertificateStoreMock.Receiver2PublicCertificate,
            },
            WindowsCertificateStoreMock.SenderPublicCertificate,
            new List<X509Certificate2>
            {
                WindowsCertificateStoreMock.Receiver1PrivateCertificate,
                WindowsCertificateStoreMock.Receiver2PrivateCertificate,
            });

        decryptedPlaintextBytesList.Should().HaveCount(2);
        decryptedPlaintextBytesList.All(x => x.SequenceEqual(plaintextBytes)).Should().BeTrue();
    }

    [Fact]
    public async Task ShouldThrowWithWrongReceiver()
    {
        var plaintextBytes = await ReadDummyText();

        var act = () => Roundtrip(
            plaintextBytes,
            WindowsCertificateStoreMock.SenderPrivateCertificate,
            new() { WindowsCertificateStoreMock.Receiver1PublicCertificate },
            WindowsCertificateStoreMock.SenderPublicCertificate,
            WindowsCertificateStoreMock.Receiver2PrivateCertificate);

        act.Should()
            .Throw<ReceiverNotFoundException>()
            .WithMessage("No matching receiver found for receiver certificate E800A2A563CC75C294508C831A61503DC9655E0B");
    }

    [Fact]
    public async Task ShouldThrowWithUsingReceiverPublicKeyInDecrypt()
    {
        var plaintextBytes = await ReadDummyText();

        var act = () => Roundtrip(
            plaintextBytes,
            WindowsCertificateStoreMock.SenderPrivateCertificate,
            new() { WindowsCertificateStoreMock.Receiver1PublicCertificate },
            WindowsCertificateStoreMock.SenderPublicCertificate,
            WindowsCertificateStoreMock.Receiver1PublicCertificate);

        act.Should()
            .Throw<PrivateKeyNotProvidedException>()
            .WithMessage("Private key on certificate A86F3051CEFA0535AD68267229D78779861133DD was not provided");
    }

    private byte[] Roundtrip(
        byte[] input,
        X509Certificate2 encryptSenderCert,
        List<X509Certificate2> encryptReceiverCerts,
        X509Certificate2 decryptSenderCert,
        X509Certificate2 decryptReceiverCert)
    {
        var encryptedPlaintextBytes = _encryptor.Encrypt(
            input,
            encryptSenderCert,
            encryptReceiverCerts);

        var decrypted = _decryptor.Decrypt(
            encryptedPlaintextBytes,
            decryptSenderCert,
            decryptReceiverCert);

        return decrypted.Decrypted;
    }

    private List<byte[]> Roundtrip(
        byte[] input,
        X509Certificate2 encryptSenderCert,
        List<X509Certificate2> encryptReceiverCerts,
        X509Certificate2 decryptSenderCert,
        List<X509Certificate2> decryptReceiverCerts)
    {
        var encryptedPlaintextBytes = _encryptor.Encrypt(
            input,
            encryptSenderCert,
            encryptReceiverCerts);

        return decryptReceiverCerts.ConvertAll(decryptReceiverCert => _decryptor.Decrypt(
            encryptedPlaintextBytes,
            decryptSenderCert,
            decryptReceiverCert).Decrypted);
    }
}
