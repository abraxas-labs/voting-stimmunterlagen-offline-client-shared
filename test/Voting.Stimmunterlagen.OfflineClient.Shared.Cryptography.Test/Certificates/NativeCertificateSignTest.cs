// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using FluentAssertions;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Exceptions;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Test.Mocks;
using Xunit;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Test.Certificates;

public class NativeCertificateSignTest
{
    private static readonly byte[] _plaintext = new byte[] { 5, 9, 1, 8, 2 };

    [Fact]
    public void ShouldWorkRoundtrip()
    {
        var signature = NativeMockCertificates.SenderPrivateCertificate.Sign(_plaintext);
        var verificationResult = NativeMockCertificates.SenderPublicCertificate.Verify(_plaintext, signature);
        verificationResult.Should().BeTrue();
    }

    [Fact]
    public void ShouldNotWorkWithWrongVerifyCertificate()
    {
        var signature = NativeMockCertificates.SenderPrivateCertificate.Sign(_plaintext);
        var verificationResult = NativeMockCertificates.Receiver1PrivateCertificate.Verify(_plaintext, signature);
        verificationResult.Should().BeFalse();
    }

    [Fact]
    public void ShouldThrowWithEmptySignature()
    {
        var act = () => NativeMockCertificates.SenderPublicCertificate.Verify(_plaintext, Array.Empty<byte>());
        act.Should().Throw<EmptyByteArrayException>();
    }

    [Fact]
    public void ShouldThrowWithNullSignature()
    {
        var act = () => NativeMockCertificates.SenderPublicCertificate.Verify(_plaintext, null!);
        act.Should().Throw<EmptyByteArrayException>();
    }

    [Fact]
    public void ShoulThrowOnSignWithEmptyPlaintext()
    {
        var act = () => NativeMockCertificates.SenderPrivateCertificate.Sign(Array.Empty<byte>());
        act.Should().Throw<EmptyByteArrayException>()
            .WithMessage("*data*");
    }

    [Fact]
    public void ShoulThrowOnVerifyWithEmptyPlaintext()
    {
        var signature = NativeMockCertificates.SenderPrivateCertificate.Sign(_plaintext);
        var act = () => NativeMockCertificates.SenderPrivateCertificate.Verify(Array.Empty<byte>(), signature);
        act.Should().Throw<EmptyByteArrayException>()
            .WithMessage("*data*");
    }
}
