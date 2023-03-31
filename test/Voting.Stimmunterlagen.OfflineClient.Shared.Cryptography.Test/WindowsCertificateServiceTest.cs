using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using FluentAssertions;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Mocks.Storage;
using Xunit;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Test;

public class WindowsCertificateServiceTest
{
    private readonly WindowsCertificateService _sut = new WindowsCertificateService(new WindowsCertificateStoreMock());

    [Fact]
    public void GetEncryptionCertificates()
    {
        var result = MapToTestResult(_sut.GetPrivateEncryptionCertificates());
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(new[]
        {
            ("CN=VOTING Encryption Receiver CA, OU=VOTING, O=Abraxas Informatik AG, L=SG, C=CH", true),
            ("CN=VOTING Encryption Receiver 2 CA, OU=VOTING, O=Abraxas Informatik AG, L=SG, C=CH", true),
        });
    }

    [Fact]
    public void GetSigningCertificates()
    {
        var result = MapToTestResult(_sut.GetPrivateSigningCertificates());
        result.Should().HaveCount(1);
        result.Should().BeEquivalentTo(new[] { ("CN=VOTING Signing Sender CA, OU=VOTING, O=Abraxas Informatik AG, L=SG, C=CH", true) });
    }

    private static List<(string Issuer, bool HasPrivateKey)> MapToTestResult(IEnumerable<X509Certificate2> collection) =>
        collection.Select(cert => (cert.Issuer, cert.HasPrivateKey)).ToList();
}
