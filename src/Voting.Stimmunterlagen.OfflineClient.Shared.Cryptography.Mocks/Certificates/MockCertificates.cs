// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Mocks.Certificates;

public static class MockCertificates
{
    public static byte[] Receiver1Public => CertificateFiles.receiver1_public;

    public static (byte[] Bytes, string Password) Receiver1Private => (CertificateFiles.receiver1_private, "1234567890");

    public static byte[] Receiver2Public => CertificateFiles.receiver2_public;

    public static (byte[] Bytes, string Password) Receiver2Private => (CertificateFiles.receiver2_private, "1234567890");

    public static byte[] SenderPublic => CertificateFiles.sender_public;

    public static (byte[] Bytes, string Password) SenderPrivate => (CertificateFiles.sender_private, "1234567890");

    public static byte[] KeystorePublic => CertificateFiles.keystore_public;

    public static (byte[] Bytes, string Password) KeystorePrivate => (CertificateFiles.keystore_private, "TzeVbpSsOoU4sRoJeK8LMAKw3X5hfYKf");
}
