// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Certificates;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Decryption;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Exceptions;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Test.Mocks;
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

        var decryptedCryptoFileBytes = RunDecrypt(cryptoFileBytes, BouncyCastleMockCertificates.SenderPublicCertificate, BouncyCastleMockCertificates.Receiver1PrivateCertificate);
        plaintextBytes.SequenceEqual(decryptedCryptoFileBytes).Should().BeTrue();
    }

    [Fact]
    public void ShouldThrowWithSenderCertificateNull()
    {
        var act = () => RunDecrypt(_mockBytes, null!, BouncyCastleMockCertificates.Receiver1PrivateCertificate);

        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("*senderCertificate*");
    }

    [Fact]
    public void ShouldThrowWithReceiverCertificateNull()
    {
        var act = () => RunDecrypt(_mockBytes, BouncyCastleMockCertificates.SenderPublicCertificate, null!);

        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("*receiverCertificate*");
    }

    [Fact]
    public async Task ShouldThrowWithMissingReceiverPrivateKey()
    {
        var cryptoFileBytes = await ReadDummyEncryptedR1Text();
        var act = () => RunDecrypt(cryptoFileBytes, BouncyCastleMockCertificates.SenderPublicCertificate, BouncyCastleMockCertificates.Receiver1PublicCertificate);

        act.Should()
            .Throw<PrivateKeyNotProvidedException>()
            .WithMessage("Private key on certificate 0CF20B24F8D9791354FD155347CAC7EDEB98C475 was not provided");
    }

    [Fact]
    public async Task ShouldThrowIfWrongReceiverCertificate()
    {
        var cryptoFileBytes = await ReadDummyEncryptedR1Text();
        var act = () => RunDecrypt(cryptoFileBytes, BouncyCastleMockCertificates.SenderPublicCertificate, BouncyCastleMockCertificates.Receiver2PrivateCertificate);

        act.Should()
            .Throw<ReceiverNotFoundException>()
            .WithMessage("No matching receiver found for receiver certificate 0AD9E6FE9EBFB380BB72085E1F6851ABDD398EEC");
    }

    [Fact]
    public async Task ShouldThrowIfPayloadManipulated()
    {
        var cryptoFileBytes = await ReadInvalidDummyEncryptedR1ManipulatedPayloadText();
        var act = () => RunDecrypt(cryptoFileBytes, BouncyCastleMockCertificates.SenderPublicCertificate, BouncyCastleMockCertificates.Receiver1PrivateCertificate);

        act.Should()
            .Throw<CryptographicException>()
            .WithMessage("Invalid signature for sender certificate 112F31E004C13E7F7B4A08C8F58DC1F3F21E651B");
    }

    [Fact]
    public async Task ShouldThrowIfSignatureManipulated()
    {
        var cryptoFileBytes = await ReadInvalidDummyEncryptedR1ManipulatedSignatureText();
        var act = () => RunDecrypt(cryptoFileBytes, BouncyCastleMockCertificates.SenderPublicCertificate, BouncyCastleMockCertificates.Receiver1PrivateCertificate);

        act.Should()
            .Throw<CryptographicException>()
            .WithMessage("Invalid signature for sender certificate 112F31E004C13E7F7B4A08C8F58DC1F3F21E651B");
    }

    [Fact]
    public void ShouldThrowWithEmptyCiphertext()
    {
        var act = () => RunDecrypt(Array.Empty<byte>(), BouncyCastleMockCertificates.SenderPublicCertificate, BouncyCastleMockCertificates.Receiver1PrivateCertificate);
        act.Should().Throw<EmptyByteArrayException>().WithMessage("*cryptoFileBytes*");
    }

    [Fact]
    public void ShouldThrowIfInvalidFileContentNewlineStart()
    {
        var act = () => RunDecrypt(new byte[] { 0x0A, 0x44, 0x44, 0x44 }, BouncyCastleMockCertificates.SenderPublicCertificate, BouncyCastleMockCertificates.Receiver1PrivateCertificate);

        act.Should()
            .Throw<CryptographicException>()
            .WithMessage("Invalid file");
    }

    [Fact]
    public void ShouldThrowIfInvalidFileContentNewlineEnd()
    {
        var act = () => RunDecrypt(new byte[] { 0x44, 0x44, 0x44, 0x0A }, BouncyCastleMockCertificates.SenderPublicCertificate, BouncyCastleMockCertificates.Receiver1PrivateCertificate);

        act.Should()
            .Throw<CryptographicException>()
            .WithMessage("Invalid file");
    }

    private byte[] RunDecrypt(byte[] cryptoFileBytes, ICertificate senderCert, ICertificate receiverCert)
    {
        return _decryptor.Decrypt(cryptoFileBytes, senderCert, receiverCert).Decrypted;
    }
}
