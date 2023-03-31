using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Exceptions;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Algorithms;

/// <summary>
/// A class to wrap and unwrap a key with a certificate.
/// </summary>
internal static class KeyWrapperCryptoAlgorithm
{
    private static readonly RSAEncryptionPadding _encryptionPadding = RSAEncryptionPadding.OaepSHA512;

    public static List<(X509Certificate2 Certificate, byte[] WrappedKey)> WrapKeys(byte[] key, List<X509Certificate2> certificates)
    {
        return certificates.ConvertAll(receiverCertificate => (receiverCertificate, WrapKey(key, receiverCertificate)));
    }

    public static byte[] UnwrapKey(byte[] encryptedKey, X509Certificate2 certificate)
    {
        if (encryptedKey.Length == 0)
        {
            throw new EmptyByteArrayException(nameof(encryptedKey));
        }

        var privateKey = certificate.GetRSAPrivateKey()
            ?? throw new PrivateKeyNotProvidedException(certificate);

        return privateKey.Decrypt(encryptedKey, _encryptionPadding);
    }

    private static byte[] WrapKey(byte[] key, X509Certificate2 certificate)
    {
        if (key.Length == 0)
        {
            throw new EmptyByteArrayException(nameof(key));
        }

        var publicKey = certificate.GetRSAPublicKey()
            ?? throw new PublicKeyNotProvidedException(certificate);

        return publicKey.Encrypt(key, _encryptionPadding);
    }
}
