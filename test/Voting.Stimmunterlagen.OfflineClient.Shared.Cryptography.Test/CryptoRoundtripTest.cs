// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Certificates;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Decryption;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Encryption;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Exceptions;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Test.Mocks;
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
            BouncyCastleMockCertificates.SenderPrivateCertificate,
            new() { BouncyCastleMockCertificates.Receiver1PublicCertificate },
            BouncyCastleMockCertificates.SenderPublicCertificate,
            BouncyCastleMockCertificates.Receiver1PrivateCertificate);

        var decryptedPlaintext = Encoding.UTF8.GetString(decryptedPlaintextBytes);
        decryptedPlaintext.Should().Be(plaintext);
    }

    [Fact]
    public async Task ShouldWorkWithBinaryFile()
    {
        var plaintextBytes = await ReadDummyPdf();

        var decryptedPlaintextBytes = Roundtrip(
            plaintextBytes,
            BouncyCastleMockCertificates.SenderPrivateCertificate,
            new() { BouncyCastleMockCertificates.Receiver1PublicCertificate },
            BouncyCastleMockCertificates.SenderPublicCertificate,
            BouncyCastleMockCertificates.Receiver1PrivateCertificate);

        plaintextBytes.SequenceEqual(decryptedPlaintextBytes).Should().BeTrue();
    }

    [Fact]
    public async Task ShouldWorkWithMultipleReceivers()
    {
        var plaintextBytes = await ReadDummyPdf();

        var decryptedPlaintextBytesList = Roundtrip(
            plaintextBytes,
            BouncyCastleMockCertificates.SenderPrivateCertificate,
            new List<ICertificate>
            {
                BouncyCastleMockCertificates.Receiver1PublicCertificate,
                BouncyCastleMockCertificates.Receiver2PublicCertificate,
            },
            BouncyCastleMockCertificates.SenderPublicCertificate,
            new List<ICertificate>
            {
                BouncyCastleMockCertificates.Receiver1PrivateCertificate,
                BouncyCastleMockCertificates.Receiver2PrivateCertificate,
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
            BouncyCastleMockCertificates.SenderPrivateCertificate,
            new() { BouncyCastleMockCertificates.Receiver1PublicCertificate },
            BouncyCastleMockCertificates.SenderPublicCertificate,
            BouncyCastleMockCertificates.Receiver2PrivateCertificate);

        act.Should()
            .Throw<ReceiverNotFoundException>()
            .WithMessage("No matching receiver found for receiver certificate 0AD9E6FE9EBFB380BB72085E1F6851ABDD398EEC");
    }

    [Fact]
    public async Task ShouldThrowWithUsingReceiverPublicKeyInDecrypt()
    {
        var plaintextBytes = await ReadDummyText();

        var act = () => Roundtrip(
            plaintextBytes,
            BouncyCastleMockCertificates.SenderPrivateCertificate,
            new() { BouncyCastleMockCertificates.Receiver1PublicCertificate },
            BouncyCastleMockCertificates.SenderPublicCertificate,
            BouncyCastleMockCertificates.Receiver1PublicCertificate);

        act.Should()
            .Throw<PrivateKeyNotProvidedException>()
            .WithMessage("Private key on certificate 0CF20B24F8D9791354FD155347CAC7EDEB98C475 was not provided");
    }

    private byte[] Roundtrip(
        byte[] input,
        ICertificate encryptSenderCert,
        List<ICertificate> encryptReceiverCerts,
        ICertificate decryptSenderCert,
        ICertificate decryptReceiverCert)
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
        ICertificate encryptSenderCert,
        List<ICertificate> encryptReceiverCerts,
        ICertificate decryptSenderCert,
        List<ICertificate> decryptReceiverCerts)
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
