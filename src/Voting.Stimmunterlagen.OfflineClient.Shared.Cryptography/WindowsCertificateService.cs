using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Storage;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography;

public class WindowsCertificateService
{
    private const bool ValidOnly = false; // TODO: jira VOTING-2876

    private static readonly X509KeyUsageFlags _signingCertificateKeyUsageFlags = X509KeyUsageFlags.DigitalSignature;
    private static readonly X509KeyUsageFlags _encryptionCertificateKeyUsageFlags = X509KeyUsageFlags.KeyEncipherment;

    private readonly IWindowsCertificateStore _store;

    public WindowsCertificateService(IWindowsCertificateStore store)
    {
        _store = store;
    }

    public X509Certificate2Collection GetPrivateSigningCertificates()
    {
        return GetCertificates(_signingCertificateKeyUsageFlags, true);
    }

    public X509Certificate2Collection GetPrivateEncryptionCertificates()
    {
        return GetCertificates(_encryptionCertificateKeyUsageFlags, true);
    }

    public X509Certificate2Collection GetCertificates(X509KeyUsageFlags usage, bool onlyWithPrivateKey)
    {
        var certificates = FindCertificates(usage);

        if (!onlyWithPrivateKey)
        {
            return certificates;
        }

        return new X509Certificate2Collection(certificates.Where(c => c.HasPrivateKey).ToArray());
    }

    private X509Certificate2Collection FindCertificates(X509KeyUsageFlags usage)
    {
        var x = _store.GetCertificates();
        return x.Find(X509FindType.FindByKeyUsage, usage.ToString(), ValidOnly);
    }
}
