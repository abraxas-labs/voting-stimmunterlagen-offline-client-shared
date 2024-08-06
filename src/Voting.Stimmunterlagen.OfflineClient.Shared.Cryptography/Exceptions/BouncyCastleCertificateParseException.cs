// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Exceptions;

public class BouncyCastleCertificateParseException : Exception
{
    public BouncyCastleCertificateParseException(string message, Exception ex)
        : base(message, ex)
    {
    }

    public BouncyCastleCertificateParseException(string message)
        : base(message)
    {
    }
}
