// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.IO;
using System.Linq;
using System.Text;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Exceptions;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Models;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Decryption;

internal static class CryptoFileBuilder
{
    private static readonly string[] _enabledVersions = new[] { CryptographyConstants.V1 };

    public static CryptoFile BuildFile(byte[] fileContentBytes)
    {
        if (fileContentBytes.Length == 0)
        {
            throw new EmptyByteArrayException(nameof(fileContentBytes));
        }

        var (textBytes, binaryBytes) = SplitTextAndBinaryBytes(fileContentBytes);

        using var msStream = new MemoryStream(textBytes);
        using var textReader = new StreamReader(msStream);

        var signatureLine = textReader.ReadLine()
            ?? throw new CryptoFileException("Expected signature but found null");

        var content = BuildContent(textReader, binaryBytes);

        return new()
        {
            Signature = signatureLine,
            Content = content,
        };
    }

    private static CryptoFileContent BuildContent(StreamReader streamReader, byte[] binaryBytes)
    {
        var header = ReadHeader(streamReader);
        var payload = BuildPayload(binaryBytes);

        return new()
        {
            Header = header,
            Payload = payload,
        };
    }

    private static CryptoFilePayload BuildPayload(byte[] binaryBytes)
    {
        var nonce = new byte[CryptographyConstants.NonceSize];
        var tag = new byte[CryptographyConstants.TagSize];
        var ciphertext = new byte[binaryBytes.Length - CryptographyConstants.NonceSize - CryptographyConstants.TagSize];

        var offset = 0;
        Buffer.BlockCopy(binaryBytes, offset, nonce, 0, nonce.Length);
        offset += nonce.Length;
        Buffer.BlockCopy(binaryBytes, offset, tag, 0, tag.Length);
        offset += tag.Length;
        Buffer.BlockCopy(binaryBytes, offset, ciphertext, 0, ciphertext.Length);

        return new()
        {
            Nonce = nonce,
            Tag = tag,
            Ciphertext = ciphertext,
        };
    }

    private static (byte[] TextBytes, byte[] BinaryBytes) SplitTextAndBinaryBytes(byte[] fileContentBytes)
    {
        var matchingDelimiterCharsCount = 0;
        var delimiter = Encoding.UTF8.GetBytes(CryptographyConstants.NewLine + CryptographyConstants.HeaderEndDelimiter + CryptographyConstants.NewLine);

        for (var i = 0; i < fileContentBytes.Length; i++)
        {
            if (fileContentBytes[i] == delimiter[matchingDelimiterCharsCount])
            {
                matchingDelimiterCharsCount++;
            }
            else
            {
                matchingDelimiterCharsCount = 0;
            }

            // found delimiter
            if (matchingDelimiterCharsCount == delimiter.Length)
            {
                var textBytes = new byte[i + 1];
                Buffer.BlockCopy(fileContentBytes, 0, textBytes, 0, textBytes.Length);

                var binaryBytes = new byte[fileContentBytes.Length - textBytes.Length];
                Buffer.BlockCopy(fileContentBytes, textBytes.Length, binaryBytes, 0, binaryBytes.Length);

                return (textBytes, binaryBytes);
            }
        }

        throw new CryptoFileException("Did not found the delimiter between the text and binary bytes");
    }

    private static CryptoFileHeader ReadHeader(StreamReader streamReader)
    {
        var header = new CryptoFileHeader();

        var headerStartDelimiter = streamReader.ReadLine();
        if (!string.Equals(headerStartDelimiter, CryptographyConstants.HeaderStartDelimiter, StringComparison.Ordinal))
        {
            throw new CryptoFileException($"Expected header start but found '{headerStartDelimiter}'");
        }

        var versionLine = streamReader.ReadLine();
        if (!_enabledVersions.Contains(versionLine))
        {
            throw new CryptoFileException($"Expected an enabled version but found '{versionLine}'");
        }

        header.Version = versionLine!;

        var senderLine = streamReader.ReadLine();
        if (string.IsNullOrWhiteSpace(senderLine) || !senderLine.StartsWith(CryptographyConstants.SenderStartDelimiter, StringComparison.Ordinal))
        {
            throw new CryptoFileException($"Expected sender line but found '{senderLine}'");
        }

        header.Sender = BuildSender(senderLine);

        var reachedHeaderEnd = false;
        string? receiverLine;

        while ((receiverLine = streamReader.ReadLine()) != null && !reachedHeaderEnd)
        {
            if (string.Equals(receiverLine, CryptographyConstants.HeaderEndDelimiter, StringComparison.Ordinal))
            {
                reachedHeaderEnd = true;
                break;
            }

            if (string.IsNullOrWhiteSpace(receiverLine) || !receiverLine.StartsWith(CryptographyConstants.ReceiverStartDelimiter, StringComparison.Ordinal))
            {
                throw new CryptoFileException($"Expected receiver line but found '{receiverLine}'");
            }

            header.Receivers.Add(BuildReceiver(receiverLine));
        }

        if (header.Receivers.Count == 0)
        {
            throw new CryptoFileException("Expected receivers but found 0");
        }

        if (!reachedHeaderEnd)
        {
            throw new CryptoFileException($"Expected header end delimiter but not found");
        }

        return header;
    }

    private static CryptoFileSender BuildSender(string senderLine)
    {
        var senderSegments = senderLine.Split(CryptographyConstants.SenderSegmentDelimiter);
        if (senderSegments.Length != CryptographyConstants.SenderSegmentCount)
        {
            throw new CryptoFileException($"Expected {CryptographyConstants.SenderSegmentCount} sender segments but found {senderSegments.Length}");
        }

        return new()
        {
            Id = senderSegments[CryptographyConstants.SenderIdSegmentIndex],
            Salt = senderSegments[CryptographyConstants.SenderSaltSegmentIndex],
        };
    }

    private static CryptoFileReceiver BuildReceiver(string receiverLine)
    {
        var receiverSegments = receiverLine.Split(CryptographyConstants.ReceiverSegmentDelimiter);
        if (receiverSegments.Length != CryptographyConstants.ReceiverSegmentCount)
        {
            throw new CryptoFileException($"Expected {CryptographyConstants.ReceiverSegmentCount} receiver segments but found {receiverSegments.Length}");
        }

        return new()
        {
            Id = receiverSegments[CryptographyConstants.ReceiverIdSegmentIndex],
            Salt = receiverSegments[CryptographyConstants.ReceiverSaltSegmentIndex],
            Payload = receiverSegments[CryptographyConstants.ReceiverPayloadSegmentIndex],
        };
    }
}
