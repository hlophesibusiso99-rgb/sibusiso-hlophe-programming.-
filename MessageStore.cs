using System.Text.Json;

namespace QuickChat;

/// <summary>Saves messages marked "Store" to a JSON file so they can be sent later.</summary>
public class MessageStore
{
    private readonly string _path;

    public MessageStore(string path = "stored_messages.json")
    {
        _path = path;
    }

    public void StoreMessage(Message message)
    {
        var stored = Load();
        stored.Add(new StoredMessage(message.MessageId, message.MessageHash, message.Recipient, message.Content));

        string json = JsonSerializer.Serialize(stored, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_path, json);
    }

    public List<StoredMessage> Load()
    {
        if (!File.Exists(_path))
        {
            return new List<StoredMessage>();
        }

        try
        {
            return JsonSerializer.Deserialize<List<StoredMessage>>(File.ReadAllText(_path))
                   ?? new List<StoredMessage>();
        }
        catch (JsonException)
        {
            // An empty or damaged file starts a fresh list instead of crashing the app.
            return new List<StoredMessage>();
        }
    }
}

public record StoredMessage(string MessageId, string MessageHash, string Recipient, string Content);
