using System;
using FluentAssertions;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Algorithms;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Exceptions;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Mocks.Storage;
using Xunit;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Test.Algorithms;

public class SignAlgorithmTest
{
    private static readonly byte[] _plaintext = new byte[] { 5, 9, 1, 8, 2 };

    [Fact]
    public void ShouldWorkRoundtrip()
    {
        var signature = SignAlgorithm.Sign(WindowsCertificateStoreMock.SenderPrivateCertificate, _plaintext);
        var verificationResult = SignAlgorithm.Verify(WindowsCertificateStoreMock.SenderPublicCertificate, _plaintext, signature);
        verificationResult.Should().BeTrue();
    }

    [Fact]
    public void ShouldNotWorkWithWrongVerifyCertificate()
    {
        var signature = SignAlgorithm.Sign(WindowsCertificateStoreMock.SenderPrivateCertificate, _plaintext);
        var verificationResult = SignAlgorithm.Verify(WindowsCertificateStoreMock.Receiver1PrivateCertificate, _plaintext, signature);
        verificationResult.Should().BeFalse();
    }

    [Fact]
    public void ShouldNotWorkWithEmptySignature()
    {
        var verificationResult = SignAlgorithm.Verify(WindowsCertificateStoreMock.SenderPublicCertificate, _plaintext, Array.Empty<byte>());
        verificationResult.Should().BeFalse();
    }

    [Fact]
    public void ShouldThrowWithNullSignature()
    {
        var act = () => SignAlgorithm.Verify(WindowsCertificateStoreMock.SenderPublicCertificate, _plaintext, null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ShoulThrowOnSignWithEmptyPlaintext()
    {
        var act = () => SignAlgorithm.Sign(WindowsCertificateStoreMock.SenderPrivateCertificate, Array.Empty<byte>());
        act.Should().Throw<EmptyByteArrayException>()
            .WithMessage("*data*");
    }

    [Fact]
    public void ShoulThrowOnVerifyWithEmptyPlaintext()
    {
        var signature = SignAlgorithm.Sign(WindowsCertificateStoreMock.SenderPrivateCertificate, _plaintext);
        var act = () => SignAlgorithm.Verify(WindowsCertificateStoreMock.SenderPrivateCertificate, Array.Empty<byte>(), signature);
        act.Should().Throw<EmptyByteArrayException>()
            .WithMessage("*data*");
    }
}
