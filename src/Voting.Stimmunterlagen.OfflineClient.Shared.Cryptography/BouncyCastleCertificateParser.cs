// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.IO;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Certificates;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Exceptions;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography;

/// <summary>
/// Contains helper methods for parsing certificates into its Bouncy Castle equivalents.
/// </summary>
public static class BouncyCastleCertificateParser
{
    public static BouncyCastleCertificate ParsePemCertificate(byte[] certificateBytes)
    {
        try
        {
            var certParser = new X509CertificateParser();
            var cert = certParser.ReadCertificate(certificateBytes);
            return new BouncyCastleCertificate(cert, cert.GetPublicKey(), null);
        }
        catch (Exception ex)
        {
            throw new BouncyCastleCertificateParseException("Error during attempt to read the pem certificate", ex);
        }
    }

    public static BouncyCastleCertificate ParseP12Certificate(byte[] certificateBytes, string certificatePassword)
    {
        try
        {
            var pkcs12Store = new Pkcs12StoreBuilder().Build();
            pkcs12Store.Load(new MemoryStream(certificateBytes), certificatePassword.ToCharArray());

            foreach (var alias in pkcs12Store.Aliases)
            {
                if (pkcs12Store.IsKeyEntry(alias))
                {
                    var privateKey = pkcs12Store.GetKey(alias).Key;
                    var cert = pkcs12Store.GetCertificate(alias).Certificate;
                    return new BouncyCastleCertificate(cert, cert.GetPublicKey(), privateKey);
                }
            }

            throw new BouncyCastleCertificateParseException("Private key not found");
        }
        catch (Exception ex)
        {
            throw new BouncyCastleCertificateParseException("Error during attempt to read the p12 certificate", ex);
        }
    }
}
