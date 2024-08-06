// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Exceptions;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Certificates;

/// <summary>
/// A certificate that processes cryptographic operations based on the native X509Certificate2.
/// Is required in particular for the use of certificates from the Windows Certificate Store.
/// </summary>
public class NativeCertificate : ICertificate
{
    private static readonly HashAlgorithmName _hashAlgorithmName = HashAlgorithmName.SHA512;
    private static readonly RSASignaturePadding _signaturePadding = RSASignaturePadding.Pss;
    private static readonly RSAEncryptionPadding _encryptionPadding = RSAEncryptionPadding.OaepSHA512;

    private readonly X509Certificate2 _certificate;

    public NativeCertificate(X509Certificate2 certificate)
    {
        _certificate = certificate;
    }

    public string Thumbprint => _certificate.Thumbprint;

    public string Subject => _certificate.Subject;

    public string CommonName => _certificate.GetNameInfo(X509NameType.SimpleName, false);

    public DateTime ValidFrom => _certificate.NotBefore.ToUniversalTime();

    public DateTime ValidTo => _certificate.NotAfter.ToUniversalTime();

    public byte[] Sign(byte[] data)
    {
        if (data == null || data.Length == 0)
        {
            throw new EmptyByteArrayException(nameof(data));
        }

        var privateKey = _certificate.GetRSAPrivateKey()
            ?? throw new PrivateKeyNotProvidedException(Thumbprint);

        return privateKey.SignData(data, _hashAlgorithmName, _signaturePadding);
    }

    public bool Verify(byte[] data, byte[] signature)
    {
        if (data == null || data.Length == 0)
        {
            throw new EmptyByteArrayException(nameof(data));
        }

        if (signature == null || signature.Length == 0)
        {
            throw new EmptyByteArrayException(nameof(signature));
        }

        var publicKey = _certificate.GetRSAPublicKey()
            ?? throw new PublicKeyNotProvidedException(Thumbprint);

        return publicKey.VerifyData(data, signature, _hashAlgorithmName, _signaturePadding);
    }

    public byte[] Encrypt(byte[] plaintext)
    {
        if (plaintext == null || plaintext.Length == 0)
        {
            throw new EmptyByteArrayException(nameof(plaintext));
        }

        var publicKey = _certificate.GetRSAPublicKey()
            ?? throw new PublicKeyNotProvidedException(Thumbprint);

        return publicKey.Encrypt(plaintext, _encryptionPadding);
    }

    public byte[] Decrypt(byte[] ciphertext)
    {
        if (ciphertext == null || ciphertext.Length == 0)
        {
            throw new EmptyByteArrayException(nameof(ciphertext));
        }

        var privateKey = _certificate.GetRSAPrivateKey()
            ?? throw new PrivateKeyNotProvidedException(Thumbprint);

        return privateKey.Decrypt(ciphertext, _encryptionPadding);
    }
}
