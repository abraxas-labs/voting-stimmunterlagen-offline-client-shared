// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Certificates;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Algorithms;

/// <summary>
/// A class to wrap and unwrap a key with a certificate.
/// </summary>
internal static class KeyWrapperCryptoAlgorithm
{
    public static List<(ICertificate Certificate, byte[] WrappedKey)> WrapKeys(byte[] key, List<ICertificate> certificates)
    {
        return certificates.ConvertAll(receiverCertificate => (receiverCertificate, WrapKey(key, receiverCertificate)));
    }

    public static byte[] UnwrapKey(byte[] encryptedKey, ICertificate certificate)
    {
        return certificate.Decrypt(encryptedKey);
    }

    private static byte[] WrapKey(byte[] key, ICertificate certificate)
    {
        return certificate.Encrypt(key);
    }
}
