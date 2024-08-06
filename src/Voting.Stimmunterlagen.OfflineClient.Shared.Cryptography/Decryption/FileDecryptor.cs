// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Algorithms;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Certificates;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Exceptions;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Models;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Decryption;

/// <summary>
/// A class to decrypt files with the offline client crypto algorithm.
/// </summary>
public class FileDecryptor
{
    private readonly ILogger<FileDecryptor> _logger;

    public FileDecryptor(ILogger<FileDecryptor> logger)
    {
        _logger = logger;
    }

    public (string SenderId, byte[] Decrypted) Decrypt(byte[] cryptoFileBytes, ICertificate senderCertificate, ICertificate receiverCertificate)
    {
        if (cryptoFileBytes.Length == 0)
        {
            throw new EmptyByteArrayException(nameof(cryptoFileBytes));
        }

        if (senderCertificate == null)
        {
            throw new ArgumentNullException(nameof(senderCertificate));
        }

        if (receiverCertificate == null)
        {
            throw new ArgumentNullException(nameof(receiverCertificate));
        }

        _logger.LogInformation("Decrypting file ({PlainTextSize} bytes in size).", cryptoFileBytes.Length);

        ValidateFileSignature(cryptoFileBytes, senderCertificate);
        var cryptoFile = CryptoFileBuilder.BuildFile(cryptoFileBytes);
        ValidateSender(cryptoFile.Content.Header.Sender, senderCertificate);

        var receiver = FindReceiver(cryptoFile.Content.Header.Receivers, receiverCertificate)
            ?? throw new ReceiverNotFoundException(receiverCertificate.Thumbprint);

        var fileKey = KeyWrapperCryptoAlgorithm.UnwrapKey(Convert.FromBase64String(receiver.Payload), receiverCertificate);

        var decrypted = new EncryptionAlgorithm().Decrypt(
            cryptoFile.Content.Payload.Nonce,
            cryptoFile.Content.Payload.Tag,
            fileKey,
            cryptoFile.Content.Payload.Ciphertext);

        _logger.LogInformation("Output file is {OutputFileSize} bytes in size.", decrypted.Length);
        return (cryptoFile.Content.Header.Sender.Id, decrypted);
    }

    private static CryptoFileReceiver? FindReceiver(IReadOnlyCollection<CryptoFileReceiver> receivers, ICertificate receiverCertificate)
    {
        return receivers.FirstOrDefault(r =>
        {
            var saltBytes = Convert.FromBase64String(r.Salt);
            var receiverId = Convert.FromBase64String(r.Id);

            using var hmac = new HMACSHA256(saltBytes);
            var certReceiverId = hmac.ComputeHash(Encoding.UTF8.GetBytes(receiverCertificate.Thumbprint));

            return receiverId.SequenceEqual(certReceiverId);
        });
    }

    /// <summary>
    /// Ensures that the sender's private key certificate identification matches with the public key certiciate on the recipient side.
    /// The private key certificate identification is provided by the sender as part of the header metadata as defined in the cryptographic ABNF
    /// specification (see: Cryptography - VO Stimmunterlagen Offline Client). The recipient equally generates the identification from the
    /// public key certificate using a salted hash algorithm and compares it with the sender's identification.
    /// </summary>
    /// <param name="sender">The sender model containing information that uniquely identifies the sender certificate used for signing.</param>
    /// <param name="senderCertificate">The public key certificate used to verify the signature on the receiver side.</param>
    /// <exception cref="CryptographicException">The certificate on the recipient side does not match with the sender's certificate.</exception>
    private static void ValidateSender(CryptoFileSender sender, ICertificate senderCertificate)
    {
        using var hmac = new HMACSHA256(Convert.FromBase64String(sender.Salt));
        var senderId = hmac.ComputeHash(Encoding.UTF8.GetBytes(senderCertificate.Thumbprint));
        var senderIdBase64 = Convert.ToBase64String(senderId);
        if (!senderIdBase64.Equals(sender.Id, StringComparison.Ordinal))
        {
            throw new CryptographicException($"The sender id does not match with the id from the provided sender certificate {senderCertificate.Thumbprint}");
        }
    }

    private static void ValidateFileSignature(byte[] fileContent, ICertificate senderCertificate)
    {
        var pos = Array.IndexOf(fileContent, (byte)0x0A);

        if (pos <= 0 || pos == fileContent.Length - 1)
        {
            throw new CryptographicException($"Invalid file");
        }

        var signature = new byte[pos];
        var payload = new byte[fileContent.Length - pos - 1];
        Buffer.BlockCopy(fileContent, 0, signature, 0, pos);
        Buffer.BlockCopy(fileContent, pos + 1, payload, 0, payload.Length);

        var sig = Encoding.UTF8.GetString(signature);

        if (!senderCertificate.Verify(
            payload,
            Convert.FromBase64String(sig)))
        {
            throw new CryptographicException($"Invalid signature for sender certificate {senderCertificate.Thumbprint}");
        }
    }
}
