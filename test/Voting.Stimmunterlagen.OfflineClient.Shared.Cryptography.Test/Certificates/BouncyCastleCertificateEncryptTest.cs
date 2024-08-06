// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Linq;
using FluentAssertions;
using Org.BouncyCastle.Crypto;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Exceptions;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Test.Mocks;
using Xunit;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Test.Certificates;

public class BouncyCastleCertificateEncryptTest
{
    private static readonly byte[] _plaintext = new byte[] { 7, 2, 4, 6, 1, 3, 0, 0, 5 };

    [Fact]
    public void ShouldWorkRoundtrip()
    {
        var ciphertext = BouncyCastleMockCertificates.Receiver1PublicCertificate.Encrypt(_plaintext);
        var decrypted = BouncyCastleMockCertificates.Receiver1PrivateCertificate.Decrypt(ciphertext);
        decrypted.SequenceEqual(_plaintext).Should().BeTrue();
    }

    [Fact]
    public void ShouldWorkKeystoreRoundtrip()
    {
        var ciphertext = BouncyCastleMockCertificates.KeystorePublicCertificate.Encrypt(_plaintext);
        var decrypted = BouncyCastleMockCertificates.KeystorePrivateCertificate.Decrypt(ciphertext);
        decrypted.SequenceEqual(_plaintext).Should().BeTrue();
    }

    [Fact]
    public void ShouldNotWorkWithWrongDecryptCertificate()
    {
        var ciphertext = BouncyCastleMockCertificates.Receiver1PublicCertificate.Encrypt(_plaintext);

        var act = () => BouncyCastleMockCertificates.Receiver2PrivateCertificate.Decrypt(ciphertext);
        act.Should().Throw<InvalidCipherTextException>();
    }

    [Fact]
    public void ShouldThrowWithMissingPrivateKeyOnDecrypt()
    {
        var ciphertext = BouncyCastleMockCertificates.Receiver1PublicCertificate.Encrypt(_plaintext);

        var act = () => BouncyCastleMockCertificates.Receiver1PublicCertificate.Decrypt(ciphertext);
        act.Should().Throw<PrivateKeyNotProvidedException>();
    }
}
