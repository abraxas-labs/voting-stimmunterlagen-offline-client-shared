// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using FluentAssertions;
using Org.BouncyCastle.Crypto;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Algorithms;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Certificates;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Exceptions;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Test.Mocks;
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
                BouncyCastleMockCertificates.Receiver1PublicCertificate,
                BouncyCastleMockCertificates.Receiver2PublicCertificate,
            },
            new()
            {
                BouncyCastleMockCertificates.Receiver1PrivateCertificate,
                BouncyCastleMockCertificates.Receiver2PrivateCertificate,
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
                BouncyCastleMockCertificates.Receiver1PublicCertificate,
                BouncyCastleMockCertificates.Receiver2PublicCertificate,
            },
            new()
            {
                BouncyCastleMockCertificates.Receiver2PrivateCertificate,
                BouncyCastleMockCertificates.Receiver1PrivateCertificate,
            });

        act.Should().Throw<InvalidCipherTextException>();
    }

    [Fact]
    public void ShouldThrowOnWrapWithPublicCerts()
    {
        var fileKey = BuildFileKey();

        var act = () => Roundtrip(
            fileKey,
            new()
            {
                BouncyCastleMockCertificates.Receiver1PublicCertificate,
                BouncyCastleMockCertificates.Receiver2PublicCertificate,
            },
            new()
            {
                BouncyCastleMockCertificates.Receiver1PublicCertificate,
                BouncyCastleMockCertificates.Receiver2PublicCertificate,
            });

        act.Should().Throw<PrivateKeyNotProvidedException>();
    }

    [Fact]
    public void ShouldThrowOnWrapWithEmptyFileKey()
    {
        var act = () => KeyWrapperCryptoAlgorithm.WrapKeys(Array.Empty<byte>(), new()
            {
                BouncyCastleMockCertificates.Receiver1PublicCertificate,
                BouncyCastleMockCertificates.Receiver2PublicCertificate,
            });
        act.Should().Throw<EmptyByteArrayException>()
            .WithMessage("*plaintext*");
    }

    [Fact]
    public void ShouldThrowOnUnwrapWithEmptyEncryptedFileKey()
    {
        var act = () => KeyWrapperCryptoAlgorithm
            .UnwrapKey(Array.Empty<byte>(), BouncyCastleMockCertificates.Receiver1PrivateCertificate);
        act.Should().Throw<EmptyByteArrayException>()
            .WithMessage("*ciphertext*");
    }

    private byte[] BuildFileKey()
    {
        var fileKey = new byte[CryptographyConstants.FileKeySize];
        RandomNumberGenerator.Fill(fileKey);
        return fileKey;
    }

    private List<byte[]> Roundtrip(byte[] fileKey, List<ICertificate> wrapCerts, List<ICertificate> unwrapCerts)
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
