// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Certificates;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Encryption;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Exceptions;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Test.Mocks;
using Xunit;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Test.Encryption;

public class FileEncryptorTest
{
    private readonly FileEncryptor _encryptor = new(new Mock<ILogger<FileEncryptor>>().Object);
    private readonly byte[] _mockBytes = new byte[] { 5, 10, 15 };

    [Fact]
    public void ShouldWork()
    {
        var data = RunEncrypt(_mockBytes, BouncyCastleMockCertificates.SenderPrivateCertificate, BouncyCastleMockCertificates.Receiver1PublicCertificate);
        data.Should().NotBeEmpty();
    }

    [Fact]
    public void ShouldThrowWithMissingSenderPrivateKey()
    {
        var act = () => RunEncrypt(_mockBytes, BouncyCastleMockCertificates.SenderPublicCertificate, BouncyCastleMockCertificates.Receiver1PublicCertificate);

        act.Should()
            .Throw<PrivateKeyNotProvidedException>()
            .WithMessage("Private key on certificate 112F31E004C13E7F7B4A08C8F58DC1F3F21E651B was not provided");
    }

    [Fact]
    public void ShouldThrowWithEmptyPlainText()
    {
        var act = () => RunEncrypt(Array.Empty<byte>(), BouncyCastleMockCertificates.SenderPublicCertificate, BouncyCastleMockCertificates.Receiver1PublicCertificate);

        act.Should()
            .Throw<EmptyByteArrayException>()
            .WithMessage("*plaintext*");
    }

    [Fact]
    public void ShouldThrowWithSenderCertificateNull()
    {
        var act = () => RunEncrypt(_mockBytes, null!, BouncyCastleMockCertificates.Receiver1PrivateCertificate);

        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("*senderCertificate*");
    }

    [Fact]
    public void ShouldThrowWithReceiverCertificateNull()
    {
        var act = () => RunEncrypt(_mockBytes, BouncyCastleMockCertificates.SenderPublicCertificate, null!);

        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("*receiverCertificates*");
    }

    private byte[] RunEncrypt(byte[] plaintextBytes, ICertificate senderCert, ICertificate receiverCert)
    {
        return _encryptor.Encrypt(plaintextBytes, senderCert, new() { receiverCert });
    }
}
