// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using FluentAssertions;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Test.Mocks;
using Xunit;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Test.Certificates;

public class MixedCertificateSignTest
{
    private static readonly byte[] _plaintext = new byte[] { 5, 9, 1, 8, 2 };

    [Fact]
    public void ShouldWorkRoundtrip()
    {
        var signature = NativeMockCertificates.SenderPrivateCertificate.Sign(_plaintext);
        var verificationResult = BouncyCastleMockCertificates.SenderPublicCertificate.Verify(_plaintext, signature);
        verificationResult.Should().BeTrue();

        signature = BouncyCastleMockCertificates.SenderPrivateCertificate.Sign(_plaintext);
        verificationResult = NativeMockCertificates.SenderPublicCertificate.Verify(_plaintext, signature);
        verificationResult.Should().BeTrue();
    }
}
