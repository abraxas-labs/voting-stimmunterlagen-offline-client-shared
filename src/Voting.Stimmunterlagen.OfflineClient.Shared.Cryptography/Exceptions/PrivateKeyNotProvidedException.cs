// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Exceptions;

public class PrivateKeyNotProvidedException : Exception
{
    public PrivateKeyNotProvidedException(string thumbprint)
        : base($"Private key on certificate {thumbprint} was not provided")
    {
    }
}
