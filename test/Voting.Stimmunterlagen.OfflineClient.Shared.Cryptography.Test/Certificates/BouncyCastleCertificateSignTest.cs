// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using FluentAssertions;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Exceptions;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Test.Mocks;
using Xunit;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Test.Certificates;

public class BouncyCastleCertificateSignTest
{
    private static readonly byte[] _plaintext = new byte[] { 5, 9, 1, 8, 2 };

    [Fact]
    public void ShouldWorkRoundtrip()
    {
        var signature = BouncyCastleMockCertificates.SenderPrivateCertificate.Sign(_plaintext);
        var verificationResult = BouncyCastleMockCertificates.SenderPublicCertificate.Verify(_plaintext, signature);
        verificationResult.Should().BeTrue();
    }

    [Fact]
    public void ShouldNotWorkWithWrongVerifyCertificate()
    {
        var signature = BouncyCastleMockCertificates.SenderPrivateCertificate.Sign(_plaintext);
        var verificationResult = BouncyCastleMockCertificates.Receiver1PrivateCertificate.Verify(_plaintext, signature);
        verificationResult.Should().BeFalse();
    }

    [Fact]
    public void ShouldThrowWithEmptySignature()
    {
        var act = () => BouncyCastleMockCertificates.SenderPublicCertificate.Verify(_plaintext, Array.Empty<byte>());
        act.Should().Throw<EmptyByteArrayException>();
    }

    [Fact]
    public void ShoulThrowOnSignWithEmptyPlaintext()
    {
        var act = () => BouncyCastleMockCertificates.SenderPrivateCertificate.Sign(Array.Empty<byte>());
        act.Should().Throw<EmptyByteArrayException>()
            .WithMessage("*data*");
    }

    [Fact]
    public void ShoulThrowOnVerifyWithEmptyPlaintext()
    {
        var signature = BouncyCastleMockCertificates.SenderPrivateCertificate.Sign(_plaintext);
        var act = () => BouncyCastleMockCertificates.SenderPrivateCertificate.Verify(Array.Empty<byte>(), signature);
        act.Should().Throw<EmptyByteArrayException>()
            .WithMessage("*data*");
    }
}
