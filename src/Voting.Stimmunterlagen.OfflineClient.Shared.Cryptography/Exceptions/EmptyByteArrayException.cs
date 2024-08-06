// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Exceptions;

public class EmptyByteArrayException : Exception
{
    public EmptyByteArrayException(string propertyName)
        : base($"Empty {propertyName} is not allowed")
    {
    }
}
