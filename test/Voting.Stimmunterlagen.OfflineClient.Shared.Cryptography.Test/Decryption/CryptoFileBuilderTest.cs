// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Decryption;
using Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Exceptions;
using Xunit;

namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography.Test.Decryption;

public class CryptoFileBuilderTest : BaseTest
{
    [Fact]
    public async Task ShouldWorkWithMatchingReceiver()
    {
        var cryptoFileBytes = await ReadDummyEncryptedR1Text();

        var cryptoFile = CryptoFileBuilder.BuildFile(cryptoFileBytes);

        Assert.Equal("YYLSpm9Cp909RKkqaCQubJaWnYyF1JLJGXpp+TDsBNTBmLNPKsvgOClzXMITPNRvrA8U5agdu1Ss52NqjoYd9BEAHrpUgy1m2nX1LXkdKz+Kiv1Shk0cEXQX1on9SkfosWJjHcXNB85uSBkuBYZqX1DKOa9wOdy931VpuV3urY1i3XLO1mdrz8b1+IHmtPRFLpJ+ZGnzJw+vKzgDxnc4z+mtIxuJM7b06rb2rlR0qI+BceK5zCal2yMc6H1W9MHpbcFKGuchRYjLiDhaalryXBC3AwhO8ZVtH58+Jd7D0oBfFZ8MsMIX4oetpDa6MkMbC0LzI9ghGJDDA8h8PyWF5TqTObiGjsVGLLzkGAVJFyw6gUvWBRtmyUdP/oU/Es/LU+QDD0gSS2sbl63q0+2v0yswtoqGqs1biG3g0T7TRZG++DDstRsdJtG8fJ2KYc6OG6Ry8ZllRNk6ukzfF+hmUGgAgoO4LqNJTh0wpFqnwUs81c8suqUD8D+L2nAgO0L1", cryptoFile.Signature);
        Assert.Equal("01", cryptoFile.Content.Header.Version);
        Assert.Equal("j2ABUHRl+pM+U7XOzHKV5ZjykL/UW/XNFIsKTEiSTZw=", cryptoFile.Content.Header.Sender.Id);
        Assert.Equal("plCA2sXFQWqe7cCMAuIYtw==", cryptoFile.Content.Header.Sender.Salt);

        Assert.Single(cryptoFile.Content.Header.Receivers);
        Assert.Equal("2CFlr37IGKzn3ztHvd9yyWX8NSQLpi1svKMfbB5YXAE=", cryptoFile.Content.Header.Receivers[0].Id);
        Assert.Equal("o7MgoURXjMFgUFHMFoYuHQ==", cryptoFile.Content.Header.Receivers[0].Salt);
        Assert.Equal("AoSSXlWZ2IrFUgfY56nrYAUn8R6B6MvfsXhKDCwlxrexMc+xthvbBtETy0YSEZ1de4F2LPzEobrwWwujEV9BXHuywFCEHxk0XtWZ11fS6GzA/Z1hB1R/0RY8gyIE3ZPAKujS6ScM3DtdtBbtYYXBs+5akmnpflU2Yet5IjjypOdNDJ+Uloo2nsvrtqZyN3MStteN87cELlCabWdnTJYt8OvloMDaQfRkVMUrP+2UK/P26DBQK9hhGAxD3nuBM+WytNgkYh9gOLKjMFEsqGK9qBORAiMA1wJbVsjaI/smr7Hf2gBDNtRWNCcCOKO5nfjiszmy7nVF4imphWlzWR1UnA==", cryptoFile.Content.Header.Receivers[0].Payload);
        Assert.Equal("n6B06E6KdE5FEUM8", Convert.ToBase64String(cryptoFile.Content.Payload.Nonce));
        Assert.Equal("aI/QxwvdY2i7lJc6dRUJ8A==", Convert.ToBase64String(cryptoFile.Content.Payload.Tag));
        Assert.Equal("v6mr1qXco6dracJgXnyMgIMgEOP4C0C/N24=", Convert.ToBase64String(cryptoFile.Content.Payload.Ciphertext));
    }

    [Fact]
    public async Task ShouldThrowIfHeaderEndIsMissing()
    {
        var cryptoFileBytes = await ReadInvalidDummyEncryptedR1WrongHeaderEndText();
        var act = () => CryptoFileBuilder.BuildFile(cryptoFileBytes);

        act.Should()
            .Throw<CryptoFileException>()
            .WithMessage("Did not found the delimiter between the text and binary bytes");
    }

    [Fact]
    public async Task ShouldThrowIfHeaderStartIsMissing()
    {
        var cryptoFileBytes = await ReadInvalidDummyEncryptedR1WrongHeaderStartText();
        var act = () => CryptoFileBuilder.BuildFile(cryptoFileBytes);

        act.Should()
            .Throw<CryptoFileException>()
            .WithMessage("Expected header start but found '-*- HS'");
    }

    [Fact]
    public async Task ShouldThrowIfNoVersion()
    {
        var cryptoFileBytes = await ReadInvalidDummyEncryptedR1NoVersionText();
        var act = () => CryptoFileBuilder.BuildFile(cryptoFileBytes);

        act.Should()
            .Throw<CryptoFileException>()
            .WithMessage("Expected an enabled version but found '<-*");
    }

    [Fact]
    public async Task ShouldThrowIfInvalidVersion()
    {
        var cryptoFileBytes = await ReadInvalidDummyEncryptedR1InvalidVersionText();
        var act = () => CryptoFileBuilder.BuildFile(cryptoFileBytes);

        act.Should()
            .Throw<CryptoFileException>()
            .WithMessage("Expected an enabled version but found '0a1'");
    }

    [Fact]
    public async Task ShouldThrowIfNoSenderInHeader()
    {
        var cryptoFileBytes = await ReadInvalidDummyEncryptedR1NoSenderText();
        var act = () => CryptoFileBuilder.BuildFile(cryptoFileBytes);

        act.Should()
            .Throw<CryptoFileException>()
            .WithMessage("Expected sender line but found '->*");
    }

    [Fact]
    public async Task ShouldThrowIfNoReceiversInHeader()
    {
        var cryptoFileBytes = await ReadInvalidDummyEncryptedR1NoReceiversText();
        var act = () => CryptoFileBuilder.BuildFile(cryptoFileBytes);

        act.Should()
            .Throw<CryptoFileException>()
            .WithMessage("Expected receivers but found 0");
    }

    [Fact]
    public async Task ShouldThrowIfSenderInvalid()
    {
        var cryptoFileBytes = await ReadInvalidDummyEncryptedR1InvalidSenderText();
        var act = () => CryptoFileBuilder.BuildFile(cryptoFileBytes);

        act.Should()
            .Throw<CryptoFileException>()
            .WithMessage("Expected sender line but found '<a-*");
    }

    [Fact]
    public async Task ShouldThrowIfReceiverInvalid()
    {
        var cryptoFileBytes = await ReadInvalidDummyEncryptedR1InvalidReceiverText();
        var act = () => CryptoFileBuilder.BuildFile(cryptoFileBytes);

        act.Should()
            .Throw<CryptoFileException>()
            .WithMessage("Expected receiver line but found '-a>*");
    }

    [Fact]
    public async Task ShouldThrowIfReceiverIsLeadingSender()
    {
        var cryptoFileBytes = await ReadInvalidDummyEncryptedR1ReceiverLeadingSenderText();
        var act = () => CryptoFileBuilder.BuildFile(cryptoFileBytes);

        act.Should()
            .Throw<CryptoFileException>()
            .WithMessage("Expected sender line but found '->*");
    }

    [Fact]
    public void ShouldThrowWithEmptyCiphertext()
    {
        var act = () => CryptoFileBuilder.BuildFile(Array.Empty<byte>());
        act.Should().Throw<EmptyByteArrayException>().WithMessage("Empty fileContentBytes is not allowed");
    }
}
