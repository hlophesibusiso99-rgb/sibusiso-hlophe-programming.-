namespace QuickChat;

/// <summary>Holds the messages sent during this run and creates unique message IDs.</summary>
public class MessageLog
{
    private readonly List<Message> _sent = new();
    private readonly HashSet<string> _usedIds = new();

    /// <summary>Returns a random ten-digit ID that has not been used in this run.</summary>
    public string GenerateMessageId()
    {
        string id;
        do
        {
            id = Random.Shared.NextInt64(1_000_000_000L, 10_000_000_000L).ToString();
        } while (!_usedIds.Add(id));

        return id;
    }

    public void Add(Message message) => _sent.Add(message);

    /// <summary>The number of messages sent so far in this run.</summary>
    public int ReturnTotalMessages() => _sent.Count;

    /// <summary>All messages sent in this run, each shown with its details in the required order.</summary>
    public string PrintMessages()
    {
        if (_sent.Count == 0)
        {
            return "No messages have been sent yet.";
        }

        string separator = Environment.NewLine + Environment.NewLine;
        return string.Join(separator, _sent.Select(m => FormatDetails(m)));
    }

    /// <summary>Message ID, Message Hash, Recipient, then Message, one per line.</summary>
    public static string FormatDetails(Message message) =>
        string.Join(Environment.NewLine,
            $"Message ID: {message.MessageId}",
            $"Message Hash: {message.MessageHash}",
            $"Recipient: {message.Recipient}",
            $"Message: {message.Content}");
}
