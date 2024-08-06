// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Certificates;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Mocks.Storage;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Test.Mocks;

public static class NativeMockCertificates
{
    public static NativeCertificate Receiver1PublicCertificate => new NativeCertificate(WindowsCertificateStoreMock.Receiver1PublicCertificate);

    public static NativeCertificate Receiver1PrivateCertificate => new NativeCertificate(WindowsCertificateStoreMock.Receiver1PrivateCertificate);

    public static NativeCertificate Receiver2PublicCertificate => new NativeCertificate(WindowsCertificateStoreMock.Receiver2PublicCertificate);

    public static NativeCertificate Receiver2PrivateCertificate => new NativeCertificate(WindowsCertificateStoreMock.Receiver2PrivateCertificate);

    public static NativeCertificate SenderPublicCertificate => new NativeCertificate(WindowsCertificateStoreMock.SenderPublicCertificate);

    public static NativeCertificate SenderPrivateCertificate => new NativeCertificate(WindowsCertificateStoreMock.SenderPrivateCertificate);
}
