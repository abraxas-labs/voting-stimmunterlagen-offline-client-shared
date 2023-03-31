using Microsoft.Extensions.DependencyInjection.Extensions;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Mocks.Storage;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Storage;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Service collection extensions for cryptography mocks.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds a windows certificate store mock.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection instance.</returns>
    public static IServiceCollection AddWindowsCertificateStoreMock(this IServiceCollection services)
    {
        return services
            .RemoveAll<IWindowsCertificateStore>()
            .AddSingleton<WindowsCertificateStoreMock>()
            .AddSingleton<IWindowsCertificateStore>(sp => sp.GetRequiredService<WindowsCertificateStoreMock>());
    }
}
