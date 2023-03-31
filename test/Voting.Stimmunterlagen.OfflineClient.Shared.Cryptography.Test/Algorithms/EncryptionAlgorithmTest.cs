using System;
using System.Linq;
using System.Security.Cryptography;
using FluentAssertions;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Algorithms;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Exceptions;
using Xunit;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Test.Algorithms;

public class EncryptionAlgorithmTest
{
    private static readonly byte[] _plaintext = new byte[] { 2, 4, 1, 3 };

    [Fact]
    public void ShouldWorkRoundtrip()
    {
        var fileKey = BuildFileKey();
        var encryptResult = EncryptionAlgorithm.Encrypt(fileKey, _plaintext);
        var decryptedBytes = EncryptionAlgorithm.Decrypt(encryptResult.Nonce, encryptResult.Tag, fileKey, encryptResult.Ciphertext);

        encryptResult.Ciphertext.SequenceEqual(_plaintext).Should().BeFalse();
        decryptedBytes.SequenceEqual(_plaintext).Should().BeTrue();
    }

    [Fact]
    public void ShouldThrowWithWrongFileKeySize()
    {
        var fileKey = new byte[3];
        RandomNumberGenerator.Fill(fileKey);
        var act = () => EncryptionAlgorithm.Encrypt(fileKey, _plaintext);
        act.Should().Throw<CryptographicException>().WithMessage("Specified key is not a valid size for this algorithm.");
    }

    [Fact]
    public void ShouldThrowWithManipulatedEncryptedBytes()
    {
        var fileKey = BuildFileKey();
        var encryptResult = EncryptionAlgorithm.Encrypt(fileKey, _plaintext);
        encryptResult.Ciphertext[^1]++;

        var act = () => EncryptionAlgorithm.Decrypt(encryptResult.Nonce, encryptResult.Tag, fileKey, encryptResult.Ciphertext);
        act.Should().Throw<CryptographicException>().WithMessage("The computed authentication tag did not match the input authentication tag.");
    }

    [Fact]
    public void ShoulThrowOnEncryptWithEmptyPlaintext()
    {
        var act = () => EncryptionAlgorithm.Encrypt(BuildFileKey(), Array.Empty<byte>());
        act.Should().Throw<EmptyByteArrayException>()
            .WithMessage("*plaintext*");
    }

    [Fact]
    public void ShoulThrowOnDecryptWithEmptyCiphertext()
    {
        var fileKey = new byte[CryptographyConstants.FileKeySize];
        RandomNumberGenerator.Fill(fileKey);

        var encryptResult = EncryptionAlgorithm.Encrypt(fileKey, _plaintext);
        var act = () => EncryptionAlgorithm.Decrypt(encryptResult.Nonce, encryptResult.Tag, fileKey, Array.Empty<byte>());
        act.Should().Throw<EmptyByteArrayException>()
            .WithMessage("*ciphertext*");
    }

    private byte[] BuildFileKey()
    {
        var fileKey = new byte[CryptographyConstants.FileKeySize];
        RandomNumberGenerator.Fill(fileKey);
        return fileKey;
    }
}
