// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Exceptions;

public class PublicKeyNotProvidedException : Exception
{
    public PublicKeyNotProvidedException(string thumbprint)
        : base($"Public key on certificate {thumbprint} was not provided")
    {
    }
}
