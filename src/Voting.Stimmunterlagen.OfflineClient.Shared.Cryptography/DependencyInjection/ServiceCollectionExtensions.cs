// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Decryption;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Encryption;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Storage;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Service collection extensions for cryptography.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds cryptography extensions for offline client cryptographic functions.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection instance.</returns>
    public static IServiceCollection AddCryptography(this IServiceCollection services)
    {
        return services
            .AddSingleton<IWindowsCertificateStore, WindowsCertificateStore>()
            .AddSingleton<WindowsCertificateService>()
            .AddSingleton<FileEncryptor>()
            .AddSingleton<FileDecryptor>();
    }
}
