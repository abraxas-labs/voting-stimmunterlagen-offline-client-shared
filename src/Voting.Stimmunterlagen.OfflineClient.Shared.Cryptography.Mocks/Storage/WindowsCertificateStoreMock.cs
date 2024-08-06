// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Security.Cryptography.X509Certificates;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Mocks.Certificates;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Storage;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Mocks.Storage;

/// <summary>
/// A mocked store to get x509 certificates.
/// </summary>
public class WindowsCertificateStoreMock : IWindowsCertificateStore
{
    private static readonly X509Certificate2Collection _collection = new()
        {
            Receiver1PublicCertificate,
            Receiver1PrivateCertificate,
            Receiver2PublicCertificate,
            Receiver2PrivateCertificate,
            SenderPrivateCertificate,
            SenderPublicCertificate,
        };

    public static X509Certificate2 Receiver1PublicCertificate => new X509Certificate2(MockCertificates.Receiver1Public);

    public static X509Certificate2 Receiver1PrivateCertificate => new X509Certificate2(MockCertificates.Receiver1Private.Bytes, MockCertificates.Receiver1Private.Password);

    public static X509Certificate2 Receiver2PublicCertificate => new X509Certificate2(MockCertificates.Receiver2Public);

    public static X509Certificate2 Receiver2PrivateCertificate => new X509Certificate2(MockCertificates.Receiver2Private.Bytes, MockCertificates.Receiver2Private.Password);

    public static X509Certificate2 SenderPublicCertificate => new X509Certificate2(MockCertificates.SenderPublic);

    public static X509Certificate2 SenderPrivateCertificate => new X509Certificate2(MockCertificates.SenderPrivate.Bytes, MockCertificates.SenderPrivate.Password);

    /// <inheritdoc />
    public X509Certificate2Collection GetCertificates() => new(_collection);
}
