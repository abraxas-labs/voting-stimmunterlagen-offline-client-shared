// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Models;

internal class CryptoFileHeader
{
    public string Version { get; set; } = string.Empty;

    public CryptoFileSender Sender { get; set; } = new();

    public List<CryptoFileReceiver> Receivers { get; set; } = new();
}
