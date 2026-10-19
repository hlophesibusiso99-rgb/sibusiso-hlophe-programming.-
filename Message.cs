using System.Text.RegularExpressions;

namespace QuickChat;

/// <summary>A chat message and the checks that apply to it.</summary>
public class Message
{
    public const int MaxLength = 250;
    public const int MaxIdLength = 10;

    // A plus sign, then a country code and the rest of the number, e.g. +27718693002
    private static readonly Regex RecipientPattern = new(@"^\+[1-9]\d{7,14}$", RegexOptions.Compiled);
    private static readonly Regex WordPattern = new(@"[\p{L}\p{N}]+", RegexOptions.Compiled);

    public Message(string messageId, int messageNumber, string recipient, string content)
    {
        MessageId = messageId;
        MessageNumber = messageNumber;
        Recipient = recipient;
        Content = content;
    }

    public string MessageId { get; }
    public int MessageNumber { get; }
    public string Recipient { get; }
    public string Content { get; }
    public string MessageHash => CreateMessageHash();

    /// <summary>True if the ID is digits only and no more than ten characters.</summary>
    public static bool CheckMessageID(string? id) =>
        !string.IsNullOrEmpty(id) && id.Length <= MaxIdLength && id.All(char.IsDigit);

    public static bool IsValidRecipient(string? number) =>
        number is not null && RecipientPattern.IsMatch(number.Trim());

    public static string CheckRecipientCell(string? number) =>
        IsValidRecipient(number)
            ? "Cell phone number successfully captured."
            : "Cell phone number is incorrectly formatted or does not contain an international code. Please correct the number and try again.";

    public static bool IsValidLength(string? text) => (text?.Length ?? 0) <= MaxLength;

    public static string CheckMessageLength(string? text)
    {
        int length = text?.Length ?? 0;

        return length <= MaxLength
            ? "Message ready to send."
            : $"Message exceeds {MaxLength} characters by {length - MaxLength}; please reduce the size.";
    }

    /// <summary>
    /// The first two digits of the ID, a colon, the message number, a colon, then the first
    /// and last words in capitals, e.g. "00:0:HITONIGHT".
    /// </summary>
    public string CreateMessageHash()
    {
        var words = WordPattern.Matches(Content).Select(m => m.Value).ToList();

        string firstAndLast = words.Count switch
        {
            0 => string.Empty,
            1 => words[0],
            _ => words[0] + words[^1],
        };

        return $"{MessageId[..2]}:{MessageNumber}:{firstAndLast.ToUpperInvariant()}";
    }

    /// <summary>Returns the result shown for a choice of Send, Disregard or Store.</summary>
    public string SentMessage(string choice) => NormalizeChoice(choice) switch
    {
        "send" => "Message successfully sent.",
        "disregard" => "Press 0 to delete the message.",
        "store" => "Message successfully stored.",
        _ => throw new ArgumentException("Choice must be Send, Disregard or Store.", nameof(choice)),
    };

    /// <summary>Maps user input to "send", "disregard" or "store". Returns null for anything else.</summary>
    public static string? NormalizeChoice(string? input) => input?.Trim().ToLowerInvariant() switch
    {
        "send" => "send",
        "disregard" or "discard" => "disregard",
        "store" => "store",
        _ => null,
    };
}
