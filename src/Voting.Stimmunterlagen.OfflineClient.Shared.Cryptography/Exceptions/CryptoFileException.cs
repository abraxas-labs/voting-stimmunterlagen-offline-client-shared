using System;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Exceptions;

public class CryptoFileException : Exception
{
    public CryptoFileException(string? message)
        : base(message)
    {
    }
}
