using System;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Exceptions;

public class ReceiverNotFoundException : Exception
{
    public ReceiverNotFoundException(string receiver)
        : base($"No matching receiver found for receiver certificate {receiver}")
    {
    }
}
