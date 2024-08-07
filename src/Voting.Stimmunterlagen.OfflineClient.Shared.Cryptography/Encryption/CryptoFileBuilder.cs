﻿// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Certificates;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Models;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Encryption;

internal static class CryptoFileBuilder
{
    public static CryptoFile BuildFile(byte[] nonce, byte[] ciphertext, byte[] tag, List<(ICertificate ReceiverCertificate, byte[] WrappedKey)> wrappedKeys, ICertificate senderCertificate)
    {
        var content = BuildContent(nonce, ciphertext, tag, senderCertificate, wrappedKeys);
        var signature = senderCertificate.Sign(FileContentConverter.ToByteArray(content));

        return new()
        {
            Content = content,
            Signature = Convert.ToBase64String(signature),
        };
    }

    private static CryptoFileContent BuildContent(byte[] nonce, byte[] ciphertext, byte[] tag, ICertificate senderCertificate, List<(ICertificate ReceiverCertificate, byte[] WrappedKey)> wrappedKeys)
    {
        return new()
        {
            Header = new()
            {
                Version = CryptographyConstants.V1,
                Sender = BuildSender(senderCertificate),
                Receivers = wrappedKeys.ConvertAll(wrappedKey => BuildReceiver(wrappedKey.ReceiverCertificate, wrappedKey.WrappedKey)),
            },
            Payload = new()
            {
                Nonce = nonce,
                Ciphertext = ciphertext,
                Tag = tag,
            },
        };
    }

    private static CryptoFileSender BuildSender(ICertificate senderCertificate)
    {
        var senderSalt = BuildSalt(CryptographyConstants.SenderSaltSize);
        using var hmac = new HMACSHA256(senderSalt);
        var senderId = hmac.ComputeHash(Encoding.UTF8.GetBytes(senderCertificate.Thumbprint));

        return new()
        {
            Id = Convert.ToBase64String(senderId),
            Salt = Convert.ToBase64String(senderSalt),
        };
    }

    private static CryptoFileReceiver BuildReceiver(ICertificate receiverCertificate, byte[] wrappedKey)
    {
        var receiverSalt = BuildSalt(CryptographyConstants.ReceiverSaltSize);
        using var hmac = new HMACSHA256(receiverSalt);
        var receiverId = hmac.ComputeHash(Encoding.UTF8.GetBytes(receiverCertificate.Thumbprint));

        return new()
        {
            Id = Convert.ToBase64String(receiverId),
            Salt = Convert.ToBase64String(receiverSalt),
            Payload = Convert.ToBase64String(wrappedKey),
        };
    }

    private static byte[] BuildSalt(int length)
    {
        if (length <= 0)
        {
            throw new InvalidOperationException($"Cannot build salt for a length of {length}");
        }

        var salt = new byte[length];
        RandomNumberGenerator.Fill(salt);
        return salt;
    }
}
