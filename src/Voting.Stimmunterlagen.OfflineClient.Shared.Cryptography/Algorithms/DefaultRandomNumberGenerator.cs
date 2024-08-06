// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Security.Cryptography;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Algorithms;

public class DefaultRandomNumberGenerator : IRandomNumberGenerator
{
    public byte[] Generate(int count)
    {
        var result = new byte[count];
        RandomNumberGenerator.Fill(result);
        return result;
    }
}
