using System;
using System.Security.Cryptography.X509Certificates;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Exceptions;

public class PublicKeyNotProvidedException : Exception
{
    public PublicKeyNotProvidedException(X509Certificate2 senderCertificate)
        : base($"Public key on certificate {senderCertificate.Thumbprint} was not provided")
    {
    }
}
