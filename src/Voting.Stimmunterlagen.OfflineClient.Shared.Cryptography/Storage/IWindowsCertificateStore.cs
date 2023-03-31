using System.Security.Cryptography.X509Certificates;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Storage;

/// <summary>
/// Interface for a store to get x509 certificates.
/// </summary>
public interface IWindowsCertificateStore
{
    /// <summary>
    /// Get all x509 certificates of the store.
    /// </summary>
    /// <returns>A x509 certificate collection.</returns>
    X509Certificate2Collection GetCertificates();
}
