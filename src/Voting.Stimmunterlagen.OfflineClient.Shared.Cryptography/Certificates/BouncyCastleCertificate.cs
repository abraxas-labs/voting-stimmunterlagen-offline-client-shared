// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Linq;
using System.Security.Cryptography;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.X509;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Exceptions;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Certificates;

/// <summary>
/// A certificate that processes cryptographic operations using Bouncy Castle.
/// </summary>
public class BouncyCastleCertificate : ICertificate
{
    private readonly X509Certificate _certificate;
    private readonly AsymmetricKeyParameter _publicKey;
    private readonly AsymmetricKeyParameter? _privateKey;

    public BouncyCastleCertificate(
        X509Certificate certificate,
        AsymmetricKeyParameter publicKey,
        AsymmetricKeyParameter? privateKey)
    {
        _certificate = certificate;
        _publicKey = publicKey;
        _privateKey = privateKey;
        Thumbprint = Convert.ToHexString(SHA1.HashData(certificate.GetEncoded()));
    }

    public string Thumbprint { get; }

    public string Subject => _certificate.SubjectDN.ToString();

    public string CommonName => _certificate.SubjectDN.GetValueList(X509Name.CN).FirstOrDefault() ?? string.Empty;

    public DateTime ValidFrom => _certificate.CertificateStructure.StartDate.ToDateTime();

    public DateTime ValidTo => _certificate.CertificateStructure.EndDate.ToDateTime();

    public byte[] Decrypt(byte[] ciphertext)
    {
        if (ciphertext.Length == 0)
        {
            throw new EmptyByteArrayException(nameof(ciphertext));
        }

        var privateKey = _privateKey
            ?? throw new PrivateKeyNotProvidedException(Thumbprint);

        var asymmetricBlockCipher = GetAsymmetricBlockCipher(privateKey, false);
        return asymmetricBlockCipher.ProcessBlock(ciphertext, 0, ciphertext.Length);
    }

    public byte[] Encrypt(byte[] plaintext)
    {
        if (plaintext == null || plaintext.Length == 0)
        {
            throw new EmptyByteArrayException(nameof(plaintext));
        }

        var publicKey = _publicKey
            ?? throw new PublicKeyNotProvidedException(Thumbprint);

        var asymmetricBlockCipher = GetAsymmetricBlockCipher(publicKey, true);
        return asymmetricBlockCipher.ProcessBlock(plaintext, 0, plaintext.Length);
    }

    public byte[] Sign(byte[] data)
    {
        if (data == null || data.Length == 0)
        {
            throw new EmptyByteArrayException(nameof(data));
        }

        var privateKey = _privateKey
            ?? throw new PrivateKeyNotProvidedException(Thumbprint);

        var signer = GetSigner(privateKey, true);
        signer.BlockUpdate(data, 0, data.Length);
        return signer.GenerateSignature();
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

        var publicKey = _publicKey
            ?? throw new PublicKeyNotProvidedException(Thumbprint);

        try
        {
            var signer = GetSigner(publicKey, false);
            signer.BlockUpdate(data, 0, data.Length);
            return signer.VerifySignature(signature);
        }
        catch (Exception)
        {
            return false;
        }
    }

    private ISigner GetSigner(AsymmetricKeyParameter key, bool forSigning)
    {
        var signer = new PssSigner(new RsaEngine(), new Sha512Digest());
        signer.Init(forSigning, key);
        return signer;
    }

    private IAsymmetricBlockCipher GetAsymmetricBlockCipher(AsymmetricKeyParameter key, bool forEncryption)
    {
        var cipher = new OaepEncoding(new RsaEngine(), new Sha512Digest());
        cipher.Init(forEncryption, key);
        return cipher;
    }
}
