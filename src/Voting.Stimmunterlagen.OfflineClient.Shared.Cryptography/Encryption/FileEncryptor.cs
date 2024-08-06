// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Algorithms;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Certificates;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Exceptions;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Encryption;

/// <summary>
/// A class to encrypt files with the offline client crypto algorithm.
/// </summary>
public class FileEncryptor
{
    private readonly ILogger<FileEncryptor> _logger;

    public FileEncryptor(ILogger<FileEncryptor> logger)
    {
        _logger = logger;
    }

    public byte[] Encrypt(byte[] plaintext, ICertificate senderCertificate, List<ICertificate> receiverCertificates)
    {
        if (plaintext.Length == 0)
        {
            throw new EmptyByteArrayException(nameof(plaintext));
        }

        if (senderCertificate == null)
        {
            throw new ArgumentNullException(nameof(senderCertificate));
        }

        if (receiverCertificates.Count(r => r != null) == 0 || receiverCertificates.Any(r => r == null))
        {
            throw new ArgumentException($"Invalid {nameof(receiverCertificates)} provided. At least one is required and only non-null are allowed");
        }

        _logger.LogInformation("Encrypting file ({InputFileSize} bytes in size).", plaintext.Length);

        var fileKey = GenerateRandomFileKey();
        var (nonce, tag, ciphertext) = new EncryptionAlgorithm().Encrypt(fileKey, plaintext);
        var wrappedKeys = KeyWrapperCryptoAlgorithm.WrapKeys(fileKey, receiverCertificates);

        var cryptoFile = CryptoFileBuilder.BuildFile(
            nonce,
            ciphertext,
            tag,
            wrappedKeys,
            senderCertificate);

        var encrypted = FileContentConverter.ToByteArray(cryptoFile);

        _logger.LogInformation("Output file is {OutputFileSize} bytes in size.", encrypted.Length);
        return encrypted;
    }

    private byte[] GenerateRandomFileKey()
    {
        var fileKey = new byte[CryptographyConstants.FileKeySize];
        RandomNumberGenerator.Fill(fileKey);
        return fileKey;
    }
}
