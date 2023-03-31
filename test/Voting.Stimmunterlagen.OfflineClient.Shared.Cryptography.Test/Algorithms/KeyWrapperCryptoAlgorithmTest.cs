using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using FluentAssertions;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Algorithms;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Exceptions;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Mocks.Storage;
using Xunit;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Test.Algorithms;

public class KeyWrapperCryptoAlgorithmTest
{
    [Fact]
    public void ShouldWorkRoundtrip()
    {
        var fileKey = BuildFileKey();

        var unwrappedFileKeys = Roundtrip(
            fileKey,
            new()
            {
                WindowsCertificateStoreMock.Receiver1PublicCertificate,
                WindowsCertificateStoreMock.Receiver2PublicCertificate,
            },
            new()
            {
                WindowsCertificateStoreMock.Receiver1PrivateCertificate,
                WindowsCertificateStoreMock.Receiver2PrivateCertificate,
            });

        foreach (var unwrappedFileKey in unwrappedFileKeys)
        {
            unwrappedFileKey.SequenceEqual(fileKey).Should().BeTrue();
        }
    }

    [Fact]
    public void ShouldNotWorkWithWrongReceiverCertificate()
    {
        var fileKey = BuildFileKey();

        var act = () => Roundtrip(
            fileKey,
            new()
            {
                WindowsCertificateStoreMock.Receiver1PublicCertificate,
                WindowsCertificateStoreMock.Receiver2PublicCertificate,
            },
            new()
            {
                WindowsCertificateStoreMock.Receiver2PrivateCertificate,
                WindowsCertificateStoreMock.Receiver1PrivateCertificate,
            });

        act.Should().Throw<CryptographicException>();
    }

    [Fact]
    public void ShouldThrowOnWrapWithPublicCerts()
    {
        var fileKey = BuildFileKey();

        var act = () => Roundtrip(
            fileKey,
            new()
            {
                WindowsCertificateStoreMock.Receiver1PublicCertificate,
                WindowsCertificateStoreMock.Receiver2PublicCertificate,
            },
            new()
            {
                WindowsCertificateStoreMock.Receiver1PublicCertificate,
                WindowsCertificateStoreMock.Receiver2PublicCertificate,
            });

        act.Should().Throw<PrivateKeyNotProvidedException>();
    }

    [Fact]
    public void ShouldThrowOnWrapWithEmptyFileKey()
    {
        var act = () => KeyWrapperCryptoAlgorithm.WrapKeys(Array.Empty<byte>(), new()
            {
                WindowsCertificateStoreMock.Receiver1PublicCertificate,
                WindowsCertificateStoreMock.Receiver2PublicCertificate,
            });
        act.Should().Throw<EmptyByteArrayException>()
            .WithMessage("*key*");
    }

    [Fact]
    public void ShouldThrowOnUnwrapWithEmptyEncryptedFileKey()
    {
        var act = () => KeyWrapperCryptoAlgorithm
            .UnwrapKey(Array.Empty<byte>(), WindowsCertificateStoreMock.Receiver1PrivateCertificate);
        act.Should().Throw<EmptyByteArrayException>()
            .WithMessage("*encryptedKey*");
    }

    private byte[] BuildFileKey()
    {
        var fileKey = new byte[CryptographyConstants.FileKeySize];
        RandomNumberGenerator.Fill(fileKey);
        return fileKey;
    }

    private List<byte[]> Roundtrip(byte[] fileKey, List<X509Certificate2> wrapCerts, List<X509Certificate2> unwrapCerts)
    {
        var result = new List<byte[]>();

        var wrapResults = KeyWrapperCryptoAlgorithm.WrapKeys(fileKey, wrapCerts);
        wrapResults.Count.Should().Be(wrapCerts.Count);

        for (var i = 0; i < wrapResults.Count; i++)
        {
            var wrapResult = wrapResults[i];
            var unwrappedFileKey = KeyWrapperCryptoAlgorithm.UnwrapKey(wrapResult.WrappedKey, unwrapCerts[i]);
            result.Add(unwrappedFileKey);
        }

        return result;
    }
}
