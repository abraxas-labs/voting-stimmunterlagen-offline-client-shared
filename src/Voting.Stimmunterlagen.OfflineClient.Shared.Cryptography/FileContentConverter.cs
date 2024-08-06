// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.IO;
using System.Text;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Models;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography;

internal static class FileContentConverter
{
    public static byte[] ToByteArray(CryptoFile fileContent)
    {
        using var ms = new MemoryStream();

        ms.Write(Encoding.UTF8.GetBytes(fileContent.Signature));
        ms.Write(Encoding.UTF8.GetBytes(CryptographyConstants.SignatureEndDelimiter));
        ms.Write(ToByteArray(fileContent.Content));

        return ms.ToArray();
    }

    public static byte[] ToByteArray(CryptoFileContent content)
    {
        using var ms = new MemoryStream();
        using var sw = new StreamWriter(ms);

        // ensure same line ending for all OS.
        sw.NewLine = CryptographyConstants.NewLine;

        // header is in utf8 format
        sw.WriteHeader(content.Header);
        sw.Flush();

        // payload is binary
        ms.Write(content.Payload.Nonce);
        ms.Write(content.Payload.Tag);
        ms.Write(content.Payload.Ciphertext);

        return ms.ToArray();
    }

    private static void WriteHeader(this StreamWriter sw, CryptoFileHeader header)
    {
        sw.WriteLine(CryptographyConstants.HeaderStartDelimiter);
        sw.WriteLine(header.Version);

        sw.WriteSender(header.Sender);

        foreach (var receiver in header.Receivers)
        {
            sw.WriteReceiver(receiver);
        }

        sw.WriteLine(CryptographyConstants.HeaderEndDelimiter);
    }

    private static void WriteSender(this StreamWriter sw, CryptoFileSender sender)
    {
        sw.Write(CryptographyConstants.SenderStartDelimiter);
        sw.WriteSpace();

        sw.Write(sender.Salt);
        sw.WriteSpace();

        sw.Write(sender.Id);
        sw.WriteLine();
    }

    private static void WriteReceiver(this StreamWriter sw, CryptoFileReceiver receiver)
    {
        sw.Write(CryptographyConstants.ReceiverStartDelimiter);
        sw.WriteSpace();

        sw.Write(receiver.Salt);
        sw.WriteSpace();

        sw.Write(receiver.Id);
        sw.WriteSpace();

        sw.Write(receiver.Payload);
        sw.WriteLine();
    }

    private static void WriteSpace(this StreamWriter sw) => sw.Write(' ');
}
