// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Linq;
using FluentAssertions;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Test.Mocks;
using Xunit;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Test.Certificates;

public class MixedCertificateEncryptTest
{
    private static readonly byte[] _plaintext = new byte[] { 5, 9, 1, 8, 2 };

    [Fact]
    public void ShouldWorkRoundtrip()
    {
        var ciphertext = NativeMockCertificates.Receiver1PublicCertificate.Encrypt(_plaintext);
        var decrypted = BouncyCastleMockCertificates.Receiver1PrivateCertificate.Decrypt(ciphertext);
        decrypted.SequenceEqual(_plaintext).Should().BeTrue();

        ciphertext = BouncyCastleMockCertificates.Receiver1PublicCertificate.Encrypt(_plaintext);
        decrypted = NativeMockCertificates.Receiver1PrivateCertificate.Decrypt(ciphertext);
        decrypted.SequenceEqual(_plaintext).Should().BeTrue();
    }
}
