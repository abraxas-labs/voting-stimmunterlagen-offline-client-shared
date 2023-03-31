using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Exceptions;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Algorithms;

/// <summary>
/// A class to sign and verify signatures.
/// </summary>
internal static class SignAlgorithm
{
    private static readonly HashAlgorithmName _hashAlgorithmName = HashAlgorithmName.SHA512;
    private static readonly RSASignaturePadding _signaturePadding = RSASignaturePadding.Pss;

    public static byte[] Sign(X509Certificate2 certificate, byte[] data)
    {
        if (data.Length == 0)
        {
            throw new EmptyByteArrayException(nameof(data));
        }

        var privateKey = certificate.GetRSAPrivateKey()
            ?? throw new PrivateKeyNotProvidedException(certificate);

        return privateKey.SignData(data, _hashAlgorithmName, _signaturePadding);
    }

    public static bool Verify(X509Certificate2 certificate, byte[] data, byte[] signature)
    {
        if (data.Length == 0)
        {
            throw new EmptyByteArrayException(nameof(data));
        }

        var publicKey = certificate.GetRSAPublicKey()
            ?? throw new PublicKeyNotProvidedException(certificate);

        return publicKey.VerifyData(data, signature, _hashAlgorithmName, _signaturePadding);
    }
}
