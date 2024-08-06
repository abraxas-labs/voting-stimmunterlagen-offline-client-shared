// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Linq;
using System.Security.Cryptography;
using FluentAssertions;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Exceptions;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Test.Mocks;
using Xunit;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Test.Certificates;

public class NativeCertificateEncryptTest
{
    private static readonly byte[] _plaintext = new byte[] { 7, 2, 4, 6, 1, 3, 0, 0, 5 };

    [Fact]
    public void ShouldWorkRoundtrip()
    {
        var ciphertext = NativeMockCertificates.Receiver1PublicCertificate.Encrypt(_plaintext);
        var decrypted = NativeMockCertificates.Receiver1PrivateCertificate.Decrypt(ciphertext);
        decrypted.SequenceEqual(_plaintext).Should().BeTrue();
    }

    [Fact]
    public void ShouldNotWorkWithWrongDecryptCertificate()
    {
        var ciphertext = NativeMockCertificates.Receiver1PublicCertificate.Encrypt(_plaintext);

        var act = () => NativeMockCertificates.Receiver2PrivateCertificate.Decrypt(ciphertext);
        act.Should().Throw<CryptographicException>();
    }

    [Fact]
    public void ShouldThrowWithMissingPrivateKeyOnDecrypt()
    {
        var ciphertext = NativeMockCertificates.Receiver1PublicCertificate.Encrypt(_plaintext);

        var act = () => NativeMockCertificates.Receiver1PublicCertificate.Decrypt(ciphertext);
        act.Should().Throw<PrivateKeyNotProvidedException>();
    }
}
