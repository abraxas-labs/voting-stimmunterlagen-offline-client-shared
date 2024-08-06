// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Certificates;

/// <summary>
/// Represents a certificate that can be used for both data signing and encryption.
/// </summary>
public interface ICertificate
{
    string Subject { get; }

    string Thumbprint { get; }

    string CommonName { get; }

    DateTime ValidFrom { get; }

    DateTime ValidTo { get; }

    byte[] Sign(byte[] data);

    bool Verify(byte[] data, byte[] signature);

    byte[] Encrypt(byte[] plaintext);

    byte[] Decrypt(byte[] ciphertext);
}
