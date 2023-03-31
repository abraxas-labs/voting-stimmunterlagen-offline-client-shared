using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Decryption;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Exceptions;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Mocks.Storage;
using Xunit;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Test.Decryption;

public class FileDecryptorTest : BaseTest
{
    private readonly byte[] _mockBytes = new byte[] { 5, 10, 15 };
    private readonly FileDecryptor _decryptor = new(new Mock<ILogger<FileDecryptor>>().Object);

    [Fact]
    public async Task ShouldWorkWithMatchingReceiver()
    {
        var plaintextBytes = await ReadDummyText();
        var cryptoFileBytes = await ReadDummyEncryptedR1Text();

        var decryptedCryptoFileBytes = RunDecrypt(cryptoFileBytes, WindowsCertificateStoreMock.SenderPublicCertificate, WindowsCertificateStoreMock.Receiver1PrivateCertificate);
        plaintextBytes.SequenceEqual(decryptedCryptoFileBytes).Should().BeTrue();
    }

    [Fact]
    public void ShouldThrowWithSenderCertificateNull()
    {
        var act = () => RunDecrypt(_mockBytes, null!, WindowsCertificateStoreMock.Receiver1PrivateCertificate);

        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("*senderCertificate*");
    }

    [Fact]
    public void ShouldThrowWithReceiverCertificateNull()
    {
        var act = () => RunDecrypt(_mockBytes, WindowsCertificateStoreMock.SenderPublicCertificate, null!);

        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("*receiverCertificate*");
    }

    [Fact]
    public async Task ShouldThrowWithMissingReceiverPrivateKey()
    {
        var cryptoFileBytes = await ReadDummyEncryptedR1Text();
        var act = () => RunDecrypt(cryptoFileBytes, WindowsCertificateStoreMock.SenderPublicCertificate, WindowsCertificateStoreMock.Receiver1PublicCertificate);

        act.Should()
            .Throw<PrivateKeyNotProvidedException>()
            .WithMessage("Private key on certificate A86F3051CEFA0535AD68267229D78779861133DD was not provided");
    }

    [Fact]
    public async Task ShouldThrowIfWrongReceiverCertificate()
    {
        var cryptoFileBytes = await ReadDummyEncryptedR1Text();
        var act = () => RunDecrypt(cryptoFileBytes, WindowsCertificateStoreMock.SenderPublicCertificate, WindowsCertificateStoreMock.Receiver2PrivateCertificate);

        act.Should()
            .Throw<ReceiverNotFoundException>()
            .WithMessage("No matching receiver found for receiver certificate E800A2A563CC75C294508C831A61503DC9655E0B");
    }

    [Fact]
    public async Task ShouldThrowIfHeaderEndIsMissing()
    {
        var cryptoFileBytes = await ReadInvalidDummyEncryptedR1WrongHeaderEndText();
        var act = () => RunDecrypt(cryptoFileBytes, WindowsCertificateStoreMock.SenderPublicCertificate, WindowsCertificateStoreMock.Receiver2PrivateCertificate);

        act.Should()
            .Throw<CryptoFileException>()
            .WithMessage("Did not found the delimiter between the text and binary bytes");
    }

    [Fact]
    public async Task ShouldThrowIfHeaderStartIsMissing()
    {
        var cryptoFileBytes = await ReadInvalidDummyEncryptedR1WrongHeaderStartText();
        var act = () => RunDecrypt(cryptoFileBytes, WindowsCertificateStoreMock.SenderPublicCertificate, WindowsCertificateStoreMock.Receiver2PrivateCertificate);

        act.Should()
            .Throw<CryptoFileException>()
            .WithMessage("Expected header start but found '-*- HS'");
    }

    [Fact]
    public async Task ShouldThrowIfPayloadManipulated()
    {
        var cryptoFileBytes = await ReadInvalidDummyEncryptedR1ManipulatedPayloadText();
        var act = () => RunDecrypt(cryptoFileBytes, WindowsCertificateStoreMock.SenderPublicCertificate, WindowsCertificateStoreMock.Receiver2PrivateCertificate);

        act.Should()
            .Throw<CryptographicException>()
            .WithMessage("Invalid signature for sender certificate D9375911620C41FD38B56F3309342D3B4BFC87A7");
    }

    [Fact]
    public async Task ShouldThrowIfNoSenderInHeader()
    {
        var cryptoFileBytes = await ReadInvalidDummyEncryptedR1NoSenderText();
        var act = () => RunDecrypt(cryptoFileBytes, WindowsCertificateStoreMock.SenderPublicCertificate, WindowsCertificateStoreMock.Receiver2PrivateCertificate);

        act.Should()
            .Throw<CryptoFileException>()
            .WithMessage("Expected sender line but found '->*");
    }

    [Fact]
    public async Task ShouldThrowIfNoReceiversInHeader()
    {
        var cryptoFileBytes = await ReadInvalidDummyEncryptedR1NoReceiversText();
        var act = () => RunDecrypt(cryptoFileBytes, WindowsCertificateStoreMock.SenderPublicCertificate, WindowsCertificateStoreMock.Receiver2PrivateCertificate);

        act.Should()
            .Throw<CryptoFileException>()
            .WithMessage("Expected receivers but found 0");
    }

    [Fact]
    public void ShouldThrowWithEmptyCiphertext()
    {
        var act = () => RunDecrypt(Array.Empty<byte>(), WindowsCertificateStoreMock.SenderPublicCertificate, WindowsCertificateStoreMock.Receiver2PrivateCertificate);
        act.Should().Throw<EmptyByteArrayException>().WithMessage("*cryptoFileBytes*");
    }

    private byte[] RunDecrypt(byte[] cryptoFileBytes, X509Certificate2 senderCert, X509Certificate2 receiverCert)
    {
        return _decryptor.Decrypt(cryptoFileBytes, senderCert, receiverCert).Decrypted;
    }
}
