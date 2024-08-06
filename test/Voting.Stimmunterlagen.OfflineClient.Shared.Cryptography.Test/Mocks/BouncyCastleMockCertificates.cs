// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Certificates;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Mocks.Certificates;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Test.Mocks;

public static class BouncyCastleMockCertificates
{
    public static BouncyCastleCertificate Receiver1PublicCertificate => BouncyCastleCertificateParser.ParsePemCertificate(MockCertificates.Receiver1Public);

    public static BouncyCastleCertificate Receiver1PrivateCertificate => BouncyCastleCertificateParser.ParseP12Certificate(MockCertificates.Receiver1Private.Bytes, MockCertificates.Receiver1Private.Password);

    public static BouncyCastleCertificate Receiver2PublicCertificate => BouncyCastleCertificateParser.ParsePemCertificate(MockCertificates.Receiver2Public);

    public static BouncyCastleCertificate Receiver2PrivateCertificate => BouncyCastleCertificateParser.ParseP12Certificate(MockCertificates.Receiver2Private.Bytes, MockCertificates.Receiver2Private.Password);

    public static BouncyCastleCertificate SenderPublicCertificate => BouncyCastleCertificateParser.ParsePemCertificate(MockCertificates.SenderPublic);

    public static BouncyCastleCertificate SenderPrivateCertificate => BouncyCastleCertificateParser.ParseP12Certificate(MockCertificates.SenderPrivate.Bytes, MockCertificates.SenderPrivate.Password);

    public static BouncyCastleCertificate KeystorePublicCertificate => BouncyCastleCertificateParser.ParsePemCertificate(MockCertificates.KeystorePublic);

    public static BouncyCastleCertificate KeystorePrivateCertificate => BouncyCastleCertificateParser.ParseP12Certificate(MockCertificates.KeystorePrivate.Bytes, MockCertificates.KeystorePrivate.Password);
}
