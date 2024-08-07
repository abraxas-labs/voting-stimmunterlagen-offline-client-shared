﻿// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Models;

internal class CryptoFilePayload
{
    public byte[] Nonce { get; set; } = Array.Empty<byte>();

    public byte[] Tag { get; set; } = Array.Empty<byte>();

    public byte[] Ciphertext { get; set; } = Array.Empty<byte>();
}
