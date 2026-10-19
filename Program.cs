using QuickChat;

var log = new MessageLog();
var store = new MessageStore();

Console.WriteLine("Welcome to QuickChat.");

if (!Login.Authenticate(maxAttempts: 3))
{
    Console.WriteLine("Too many failed attempts. Goodbye.");
    return;
}

int limit = ReadMessageLimit();
int remaining = limit;

while (true)
{
    Console.WriteLine();
    Console.WriteLine("1) Send Messages");
    Console.WriteLine("2) Show recently sent messages");
    Console.WriteLine("3) Quit");
    Console.Write("Select an option: ");

    switch (ReadInput().Trim())
    {
        case "1":
            if (remaining == 0)
            {
                Console.WriteLine($"You have already entered all {limit} message(s) for this session.");
                break;
            }

            ComposeMessage(log, store);
            remaining--;

            if (remaining == 0)
            {
                Console.WriteLine($"All messages have been entered. Total messages sent: {log.ReturnTotalMessages()}");
            }
            break;

        case "2":
            Console.WriteLine("Coming Soon.");
            break;

        case "3":
            Console.WriteLine("Goodbye.");
            return;

        default:
            Console.WriteLine("Invalid option. Please choose 1, 2 or 3.");
            break;
    }
}

// ---------- helpers ----------

static string ReadInput()
{
    // Throws if input has ended, so the loops below cannot spin forever.
    return Console.ReadLine() ?? throw new EndOfStreamException("No more input.");
}

static int ReadMessageLimit()
{
    while (true)
    {
        Console.Write("How many messages would you like to enter this session? ");
        if (int.TryParse(ReadInput(), out int value) && value > 0)
        {
            return value;
        }

        Console.WriteLine("Please enter a whole number greater than 0.");
    }
}

static void ComposeMessage(MessageLog messageLog, MessageStore messageStore)
{
    string recipient = ReadRecipient();
    string content = ReadContent();

    string id = messageLog.GenerateMessageId();
    Console.WriteLine($"Message ID generated: {id}");

    var message = new Message(id, messageLog.ReturnTotalMessages(), recipient, content);

    string choice = ReadChoice();
    Console.WriteLine(message.SentMessage(choice));

    switch (choice)
    {
        case "send":
            messageLog.Add(message);
            Console.WriteLine(MessageLog.FormatDetails(message));
            break;

        case "store":
            messageStore.StoreMessage(message);
            break;

        case "disregard":
            ReadInput(); // answers the "Press 0" prompt; any input discards the message
            break;
    }
}

static string ReadRecipient()
{
    while (true)
    {
        Console.Write("Recipient cell number (international format, e.g. +27718693002): ");
        string input = ReadInput().Trim();
        Console.WriteLine(Message.CheckRecipientCell(input));

        if (Message.IsValidRecipient(input))
        {
            return input;
        }
    }
}

static string ReadContent()
{
    while (true)
    {
        Console.Write($"Message (up to {Message.MaxLength} characters): ");
        string input = ReadInput();

        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("Message cannot be empty.");
            continue;
        }

        Console.WriteLine(Message.CheckMessageLength(input));

        if (Message.IsValidLength(input))
        {
            return input;
        }
    }
}

static string ReadChoice()
{
    while (true)
    {
        Console.Write("Choose Send, Disregard or Store: ");
        string? choice = Message.NormalizeChoice(ReadInput());

        if (choice is not null)
        {
            return choice;
        }

        Console.WriteLine("Invalid choice. Please type Send, Disregard or Store.");
    }
}
