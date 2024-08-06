// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Security.Cryptography.X509Certificates;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Storage;

/// <inheritdoc />
public class WindowsCertificateStore : IWindowsCertificateStore
{
    /// <inheritdoc />
    public X509Certificate2Collection GetCertificates()
    {
        using var store = OpenMyCurrentUserStore();
        return store.Certificates;
    }

    private static X509Store OpenMyCurrentUserStore()
    {
        var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
        store.Open(OpenFlags.ReadOnly);
        return store;
    }
}
