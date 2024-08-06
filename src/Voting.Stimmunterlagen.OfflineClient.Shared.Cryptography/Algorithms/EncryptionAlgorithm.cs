// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Security.Cryptography;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Exceptions;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Algorithms;

/// <summary>
/// A class to encrypt and decrypt data.
/// </summary>
internal class EncryptionAlgorithm
{
    private readonly IRandomNumberGenerator _randomNumberGenerator;

    public EncryptionAlgorithm()
        : this(new DefaultRandomNumberGenerator())
    {
    }

    public EncryptionAlgorithm(IRandomNumberGenerator randomNumberGenerator)
    {
        _randomNumberGenerator = randomNumberGenerator;
    }

    public (byte[] Nonce, byte[] Tag, byte[] Ciphertext) Encrypt(byte[] key, byte[] plaintext)
    {
        if (plaintext.Length == 0)
        {
            throw new EmptyByteArrayException(nameof(plaintext));
        }

        var nonce = _randomNumberGenerator.Generate(CryptographyConstants.NonceSize);
        var ciphertext = new byte[plaintext.Length];
        var tag = new byte[CryptographyConstants.TagSize];

        using var aesGcm = new AesGcm(key, CryptographyConstants.TagSize);
        aesGcm.Encrypt(nonce, plaintext, ciphertext, tag);

        return (nonce, tag, ciphertext);
    }

    public byte[] Decrypt(byte[] nonce, byte[] tag, byte[] key, byte[] ciphertext)
    {
        if (ciphertext.Length == 0)
        {
            throw new EmptyByteArrayException(nameof(ciphertext));
        }

        var plaintext = new byte[ciphertext.Length];
        using var aesGcm = new AesGcm(key, CryptographyConstants.TagSize);
        aesGcm.Decrypt(nonce, ciphertext, tag, plaintext);
        return plaintext;
    }
}
