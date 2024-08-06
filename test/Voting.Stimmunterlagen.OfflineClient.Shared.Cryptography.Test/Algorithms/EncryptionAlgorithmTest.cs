// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using FluentAssertions;
using Moq;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Algorithms;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Exceptions;
using Xunit;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Test.Algorithms;

public class EncryptionAlgorithmTest
{
    private static readonly byte[] Plaintext = Encoding.UTF8.GetBytes("This is a test string which must be more than 16 bytes and include some special characters like @ or # or even {");

    private static readonly byte[] Nonce =
    {
        0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x02, 0x02, 0x02, 0x02,
    };

    private static readonly byte[] Key =
    {
        0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04,
    };

    private static readonly byte[] Ciphertext =
    {
        0x02, 0x18, 0x4b, 0x19, 0xef, 0x4e, 0x88, 0x94, 0xca, 0xc5, 0x3f, 0x21, 0xa3, 0x92, 0xc9, 0xc1,
        0x69, 0x34, 0x97, 0xb9, 0x84, 0x96, 0xcb, 0x5f, 0x9f, 0x31, 0x01, 0x17, 0xdd, 0x5e, 0xf8, 0xb5,
        0x16, 0xe8, 0xe2, 0xfb, 0x02, 0x95, 0xe0, 0x3e, 0xd0, 0x06, 0x4a, 0xb8, 0xfc, 0x6d, 0x74, 0x3e,
        0x04, 0xa0, 0xcf, 0xbf, 0xee, 0x0a, 0x6c, 0x4c, 0x11, 0x14, 0x8c, 0x20, 0x45, 0xc0, 0xf6, 0x46,
        0x25, 0x9b, 0xda, 0x73, 0x3a, 0xd7, 0x7f, 0x0b, 0x60, 0x68, 0x79, 0xae, 0xee, 0xf9, 0x23, 0xce,
        0xa8, 0xa0, 0xf4, 0x06, 0x12, 0xf8, 0x67, 0x30, 0x1a, 0x39, 0xcd, 0x99, 0x93, 0xf1, 0xae, 0x5e,
        0x82, 0x96, 0xfb, 0xe6, 0x72, 0x36, 0xcd, 0x97, 0x6b, 0x2a, 0xf2, 0xe5, 0x1e, 0xfe, 0x25, 0xfc,
    };

    private static readonly byte[] Tag =
    {
        0x04, 0xd8, 0x62, 0x3e, 0xd5, 0x4a, 0xda, 0x14, 0x25, 0x92, 0x26, 0x18, 0x4d, 0xe3, 0x82, 0x60,
    };

    [Fact]
    public void ShouldWorkEncrypt()
    {
        var randomNumberGenerator = new Mock<IRandomNumberGenerator>();
        randomNumberGenerator.Setup(x => x.Generate(12)).Returns(Nonce);

        var encryptResult = new EncryptionAlgorithm(randomNumberGenerator.Object).Encrypt(Key, Plaintext);

        Assert.Equal(Nonce, encryptResult.Nonce);
        Assert.Equal(Ciphertext, encryptResult.Ciphertext);
        Assert.Equal(Tag, encryptResult.Tag);
        randomNumberGenerator.Verify(x => x.Generate(12), Times.Once);
        randomNumberGenerator.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldWorkDecrypt()
    {
        var decrypted = new EncryptionAlgorithm().Decrypt(Nonce, Tag, Key, Ciphertext);
        Assert.Equal(Plaintext, decrypted);
    }

    [Fact]
    public void ShouldThrowWithManipulatedCiphertext()
    {
        var manipulated = Ciphertext.ToArray();
        manipulated[5] = 0x99;
        var act = () => new EncryptionAlgorithm().Decrypt(Nonce, Tag, Key, manipulated);
        act.Should().Throw<CryptographicException>().WithMessage("The computed authentication tag did not match the input authentication tag.");
    }

    [Fact]
    public void ShouldThrowWithManipulatedKey()
    {
        var manipulated = Key.ToArray();
        manipulated[5] = 0x99;
        var act = () => new EncryptionAlgorithm().Decrypt(Nonce, Tag, manipulated, Ciphertext);
        act.Should().Throw<CryptographicException>().WithMessage("The computed authentication tag did not match the input authentication tag.");
    }

    [Fact]
    public void ShouldThrowWithManipulatedTag()
    {
        var manipulated = Tag.ToArray();
        manipulated[5] = 0x99;
        var act = () => new EncryptionAlgorithm().Decrypt(Nonce, manipulated, Key, Ciphertext);
        act.Should().Throw<CryptographicException>().WithMessage("The computed authentication tag did not match the input authentication tag.");
    }

    [Fact]
    public void ShouldThrowWithManipulatedNonce()
    {
        var manipulated = Nonce.ToArray();
        manipulated[5] = 0x99;
        var act = () => new EncryptionAlgorithm().Decrypt(manipulated, Tag, Key, Ciphertext);
        act.Should().Throw<CryptographicException>().WithMessage("The computed authentication tag did not match the input authentication tag.");
    }

    [Fact]
    public void ShouldThrowWithWrongFileKeySize()
    {
        var fileKey = new byte[3];
        RandomNumberGenerator.Fill(fileKey);
        var act = () => new EncryptionAlgorithm().Encrypt(fileKey, Plaintext);
        act.Should().Throw<CryptographicException>().WithMessage("Specified key is not a valid size for this algorithm.");
    }

    [Fact]
    public void ShoulThrowOnEncryptWithEmptyPlaintext()
    {
        var act = () => new EncryptionAlgorithm().Encrypt(Key, Array.Empty<byte>());
        act.Should().Throw<EmptyByteArrayException>()
            .WithMessage("*plaintext*");
    }

    [Fact]
    public void ShoulThrowOnDecryptWithEmptyCiphertext()
    {
        var act = () => new EncryptionAlgorithm().Decrypt(Nonce, Tag, Key, Array.Empty<byte>());
        act.Should().Throw<EmptyByteArrayException>()
            .WithMessage("*ciphertext*");
    }
}
