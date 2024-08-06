// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Algorithms;

public interface IRandomNumberGenerator
{
    byte[] Generate(int count);
}
