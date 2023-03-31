using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Extensions.Logging;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Algorithms;
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

    public (string SenderId, byte[] Decrypted) Decrypt(byte[] cryptoFileBytes, X509Certificate2 senderCertificate, X509Certificate2 receiverCertificate)
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

        var cryptoFile = CryptoFileBuilder.BuildFile(cryptoFileBytes);

        ValidateSignature(cryptoFile, senderCertificate);

        var receiver = FindReceiver(cryptoFile.Content.Header.Receivers, receiverCertificate)
            ?? throw new ReceiverNotFoundException(receiverCertificate.Thumbprint);

        var fileKey = KeyWrapperCryptoAlgorithm.UnwrapKey(Convert.FromBase64String(receiver.Payload), receiverCertificate);

        var decrypted = EncryptionAlgorithm.Decrypt(
            cryptoFile.Content.Payload.Nonce,
            cryptoFile.Content.Payload.Tag,
            fileKey,
            cryptoFile.Content.Payload.Ciphertext);

        _logger.LogInformation("Output file is {OutputFileSize} bytes in size.", decrypted.Length);
        return (cryptoFile.Content.Header.Sender.Id, decrypted);
    }

    private static CryptoFileReceiver? FindReceiver(IReadOnlyCollection<CryptoFileReceiver> receivers, X509Certificate2 receiverCertificate)
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

    private static void ValidateSignature(CryptoFile fileContent, X509Certificate2 senderCertificate)
    {
        if (!SignAlgorithm.Verify(
            senderCertificate,
            FileContentConverter.ToByteArray(fileContent.Content),
            Convert.FromBase64String(fileContent.Signature)))
        {
            throw new CryptographicException($"Invalid signature for sender certificate {senderCertificate.Thumbprint}");
        }
    }
}
