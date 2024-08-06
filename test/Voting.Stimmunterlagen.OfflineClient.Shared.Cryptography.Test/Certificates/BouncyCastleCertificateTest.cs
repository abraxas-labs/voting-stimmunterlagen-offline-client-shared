// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using FluentAssertions;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Test.Mocks;
using Xunit;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Test.Certificates;

public class BouncyCastleCertificateTest
{
    [Fact]
    public void ShouldReturnInfoWithCertificatePem()
    {
        var certificate = BouncyCastleMockCertificates.SenderPublicCertificate;
        certificate.Thumbprint.Should().Be("112F31E004C13E7F7B4A08C8F58DC1F3F21E651B");
        certificate.CommonName.Should().Be("VOTING Signing Sender");
        certificate.Subject.Should().Be("C=CH,L=SG,O=Abraxas Informatik AG,OU=VOTING,CN=VOTING Signing Sender");
        certificate.ValidFrom.Should().Be(new DateTime(2024, 6, 26, 8, 4, 54, DateTimeKind.Utc));
        certificate.ValidTo.Should().Be(new DateTime(2025, 6, 26, 8, 4, 54, DateTimeKind.Utc));
    }

    [Fact]
    public void ShouldReturnInfoWithCertificateP12Parse()
    {
        var certificate = BouncyCastleMockCertificates.SenderPrivateCertificate;
        certificate.Thumbprint.Should().Be("112F31E004C13E7F7B4A08C8F58DC1F3F21E651B");
        certificate.CommonName.Should().Be("VOTING Signing Sender");
        certificate.Subject.Should().Be("C=CH,L=SG,O=Abraxas Informatik AG,OU=VOTING,CN=VOTING Signing Sender");
        certificate.ValidFrom.Should().Be(new DateTime(2024, 6, 26, 8, 4, 54, DateTimeKind.Utc));
        certificate.ValidTo.Should().Be(new DateTime(2025, 6, 26, 8, 4, 54, DateTimeKind.Utc));
    }

    [Fact]
    public void ShouldReturnInfoWithKeystoreCertificatePemParse()
    {
        var certificate = BouncyCastleMockCertificates.KeystorePublicCertificate;
        certificate.Thumbprint.Should().Be("453D08BD1B49233137A6CAD4C1B1E091D22F0A92");
        certificate.CommonName.Should().Be("printing_component");
        certificate.Subject.Should().Be("CN=printing_component,C=Switzerland,O=DT_SG_20240621_TT01,L=St.Gallen,ST=St.Gallen");
        certificate.ValidFrom.Should().Be(new DateTime(2024, 6, 23, 22, 0, 0, DateTimeKind.Utc));
        certificate.ValidTo.Should().Be(new DateTime(2028, 6, 23, 22, 0, 0, DateTimeKind.Utc));
    }

    [Fact]
    public void ShouldReturnInfoWithKeystoreCertificateP12Parse()
    {
        var certificate = BouncyCastleMockCertificates.KeystorePrivateCertificate;
        certificate.Thumbprint.Should().Be("453D08BD1B49233137A6CAD4C1B1E091D22F0A92");
        certificate.CommonName.Should().Be("printing_component");
        certificate.Subject.Should().Be("CN=printing_component,C=Switzerland,O=DT_SG_20240621_TT01,L=St.Gallen,ST=St.Gallen");
        certificate.ValidFrom.Should().Be(new DateTime(2024, 6, 23, 22, 0, 0, DateTimeKind.Utc));
        certificate.ValidTo.Should().Be(new DateTime(2028, 6, 23, 22, 0, 0, DateTimeKind.Utc));
    }
}
