using System;
using System.Security.Cryptography.X509Certificates;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Exceptions;

public class PrivateKeyNotProvidedException : Exception
{
    public PrivateKeyNotProvidedException(X509Certificate2 senderCertificate)
        : base($"Private key on certificate {senderCertificate.Thumbprint} was not provided")
    {
    }
}
