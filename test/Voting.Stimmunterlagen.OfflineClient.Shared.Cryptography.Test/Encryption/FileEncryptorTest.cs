using System;
using System.Security.Cryptography.X509Certificates;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Encryption;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Exceptions;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Mocks.Storage;
using Xunit;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Test.Encryption;

public class FileEncryptorTest
{
    private readonly FileEncryptor _encryptor = new(new Mock<ILogger<FileEncryptor>>().Object);
    private readonly byte[] _mockBytes = new byte[] { 5, 10, 15 };

    [Fact]
    public void ShouldWork()
    {
        var data = RunEncrypt(_mockBytes, WindowsCertificateStoreMock.SenderPrivateCertificate, WindowsCertificateStoreMock.Receiver1PublicCertificate);
        data.Should().NotBeEmpty();
    }

    [Fact]
    public void ShouldThrowWithMissingSenderPrivateKey()
    {
        var act = () => RunEncrypt(_mockBytes, WindowsCertificateStoreMock.SenderPublicCertificate, WindowsCertificateStoreMock.Receiver1PublicCertificate);

        act.Should()
            .Throw<PrivateKeyNotProvidedException>()
            .WithMessage("Private key on certificate D9375911620C41FD38B56F3309342D3B4BFC87A7 was not provided");
    }

    [Fact]
    public void ShouldThrowWithEmptyPlainText()
    {
        var act = () => RunEncrypt(Array.Empty<byte>(), WindowsCertificateStoreMock.SenderPublicCertificate, WindowsCertificateStoreMock.Receiver1PublicCertificate);

        act.Should()
            .Throw<EmptyByteArrayException>()
            .WithMessage("*plaintext*");
    }

    [Fact]
    public void ShouldThrowWithSenderCertificateNull()
    {
        var act = () => RunEncrypt(_mockBytes, null!, WindowsCertificateStoreMock.Receiver1PrivateCertificate);

        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("*senderCertificate*");
    }

    [Fact]
    public void ShouldThrowWithReceiverCertificateNull()
    {
        var act = () => RunEncrypt(_mockBytes, WindowsCertificateStoreMock.SenderPublicCertificate, null!);

        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("*receiverCertificates*");
    }

    private byte[] RunEncrypt(byte[] plaintextBytes, X509Certificate2 senderCert, X509Certificate2 receiverCert)
    {
        return _encryptor.Encrypt(plaintextBytes, senderCert, new() { receiverCert });
    }
}
