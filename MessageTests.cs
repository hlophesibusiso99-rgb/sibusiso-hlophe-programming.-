using QuickChat;
using Xunit;

namespace QuickChat.Tests;

public class MessageHashTests
{
    [Fact]
    public void Hash_TestCase1_MatchesBrief()
    {
        var message = new Message("0012345678", 0, "+27718693002",
            "Hi Mike, can you join us for dinner tonight?");

        Assert.Equal("00:0:HITONIGHT", message.MessageHash);
    }

    [Fact]
    public void Hash_UsesIdPrefixAndMessageNumber()
    {
        var message = new Message("0112345678", 1, "+27718693002",
            "Hi Keegan, did you receive the payment?");

        Assert.Equal("01:1:HIPAYMENT", message.MessageHash);
    }

    [Fact]
    public void Hash_SingleWordIsNotDoubled()
    {
        var message = new Message("0012345678", 0, "+27718693002", "Hello");

        Assert.Equal("00:0:HELLO", message.MessageHash);
    }
}

public class ValidationTests
{
    [Theory]
    [InlineData("0012345678", true)]
    [InlineData("123456789", true)]
    [InlineData("12345678901", false)]
    [InlineData("12AB", false)]
    [InlineData("", false)]
    public void CheckMessageID_AcceptsDigitsUpToTenCharacters(string id, bool expected)
    {
        Assert.Equal(expected, Message.CheckMessageID(id));
    }

    [Fact]
    public void Length_250Characters_IsReadyToSend()
    {
        Assert.Equal("Message ready to send.", Message.CheckMessageLength(new string('a', 250)));
    }

    [Fact]
    public void Length_OverLimit_ReportsHowManyCharactersOver()
    {
        Assert.Equal(
            "Message exceeds 250 characters by 5; please reduce the size.",
            Message.CheckMessageLength(new string('a', 255)));
    }

    [Fact]
    public void Recipient_WithInternationalCode_IsCaptured()
    {
        Assert.Equal(
            "Cell phone number successfully captured.",
            Message.CheckRecipientCell("+27718693002"));
    }

    [Fact]
    public void Recipient_WithoutInternationalCode_IsRejected()
    {
        Assert.Equal(
            "Cell phone number is incorrectly formatted or does not contain an international code. Please correct the number and try again.",
            Message.CheckRecipientCell("08575975889"));
    }
}

public class SentMessageTests
{
    private static readonly Message Sample = new("0012345678", 0, "+27718693002", "Hi Mike");

    [Theory]
    [InlineData("Send", "Message successfully sent.")]
    [InlineData("Disregard", "Press 0 to delete the message.")]
    [InlineData("Discard", "Press 0 to delete the message.")]
    [InlineData("Store", "Message successfully stored.")]
    public void SentMessage_ReturnsResultForChoice(string choice, string expected)
    {
        Assert.Equal(expected, Sample.SentMessage(choice));
    }
}

public class MessageLogTests
{
    [Fact]
    public void GenerateMessageId_ReturnsUniqueTenDigitIds()
    {
        var log = new MessageLog();
        var seen = new HashSet<string>();

        for (int i = 0; i < 500; i++)
        {
            string id = log.GenerateMessageId();

            Assert.Equal(10, id.Length);
            Assert.True(Message.CheckMessageID(id));
            Assert.True(seen.Add(id));
        }
    }

    [Fact]
    public void ReturnTotalMessages_CountsSentMessages()
    {
        var log = new MessageLog();
        log.Add(new Message("0012345678", 0, "+27718693002", "Hi Mike"));
        log.Add(new Message("0112345678", 1, "+27718693002", "Hi Keegan"));

        Assert.Equal(2, log.ReturnTotalMessages());
    }

    [Fact]
    public void FormatDetails_ShowsIdHashRecipientAndMessageInOrder()
    {
        var message = new Message("0012345678", 0, "+27718693002",
            "Hi Mike, can you join us for dinner tonight?");

        string text = MessageLog.FormatDetails(message);

        int id = text.IndexOf("Message ID:");
        int hash = text.IndexOf("Message Hash:");
        int recipient = text.IndexOf("Recipient:");
        int body = text.IndexOf("Message: Hi Mike");

        Assert.True(id < hash && hash < recipient && recipient < body);
    }
}

public class MessageStoreTests
{
    [Fact]
    public void StoreMessage_SavesToJsonAndLoadsBack()
    {
        string path = Path.Combine(Path.GetTempPath(), $"quickchat-{Guid.NewGuid()}.json");

        try
        {
            var store = new MessageStore(path);
            store.StoreMessage(new Message("0012345678", 0, "+27718693002", "Hi Mike"));
            store.StoreMessage(new Message("0112345678", 1, "+27718693002", "Hi Keegan"));

            var saved = store.Load();

            Assert.Equal(2, saved.Count);
            Assert.Equal("0012345678", saved[0].MessageId);
            Assert.Equal("00:0:HIMIKE", saved[0].MessageHash);
        }
        finally
        {
            File.Delete(path);
        }
    }
}
