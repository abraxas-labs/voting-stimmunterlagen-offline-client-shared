// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using FluentAssertions;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Test.Mocks;
using Xunit;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Test.Certificates;

public class NativeCertificateTest
{
    [Fact]
    public void ShouldReturnInfoWithCertificatePem()
    {
        var certificate = NativeMockCertificates.SenderPublicCertificate;
        certificate.Thumbprint.Should().Be("112F31E004C13E7F7B4A08C8F58DC1F3F21E651B");
        certificate.CommonName.Should().Be("VOTING Signing Sender");
        certificate.Subject.Should().Be("CN=VOTING Signing Sender, OU=VOTING, O=Abraxas Informatik AG, L=SG, C=CH");
        certificate.ValidFrom.Should().Be(new DateTime(2024, 6, 26, 8, 4, 54, DateTimeKind.Utc));
        certificate.ValidTo.Should().Be(new DateTime(2025, 6, 26, 8, 4, 54, DateTimeKind.Utc));
    }

    [Fact]
    public void ShouldReturnInfoWithCertificateP12Parse()
    {
        var certificate = NativeMockCertificates.SenderPrivateCertificate;
        certificate.Thumbprint.Should().Be("112F31E004C13E7F7B4A08C8F58DC1F3F21E651B");
        certificate.CommonName.Should().Be("VOTING Signing Sender");
        certificate.Subject.Should().Be("CN=VOTING Signing Sender, OU=VOTING, O=Abraxas Informatik AG, L=SG, C=CH");
        certificate.ValidFrom.Should().Be(new DateTime(2024, 6, 26, 8, 4, 54, DateTimeKind.Utc));
        certificate.ValidTo.Should().Be(new DateTime(2025, 6, 26, 8, 4, 54, DateTimeKind.Utc));
    }
}
