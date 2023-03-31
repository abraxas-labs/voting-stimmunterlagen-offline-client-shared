namespace Voting.Stimmunterlagen.OfflineClient.Shared.Cryptography;

internal static class CryptographyConstants
{
    public const string V1 = "01";

    public const int FileKeySize = 32;
    public const int NonceSize = 12;
    public const int TagSize = 16;

    public const string NewLine = "\n";
    public const string SignatureEndDelimiter = NewLine;
    public const string HeaderStartDelimiter = "--- HS";
    public const string HeaderEndDelimiter = "--- HE";

    public const string SenderStartDelimiter = "<-";
    public const string ReceiverStartDelimiter = "->";

    public const string SenderSegmentDelimiter = " ";
    public const string ReceiverSegmentDelimiter = " ";

    public const int SenderSaltSize = 16;
    public const int ReceiverSaltSize = 16;

    public const int SenderSaltSegmentIndex = 1;
    public const int SenderIdSegmentIndex = 2;
    public const int SenderSegmentCount = 3;

    public const int ReceiverSaltSegmentIndex = 1;
    public const int ReceiverIdSegmentIndex = 2;
    public const int ReceiverPayloadSegmentIndex = 3;
    public const int ReceiverSegmentCount = 4;
}
