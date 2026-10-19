namespace QuickChat;

/// <summary>
/// PLACEHOLDER login. Replace the contents of this class with your Part 1 login,
/// keeping the Authenticate method so Program.cs still works.
/// </summary>
public static class Login
{
    private const string ExpectedUsername = "admin";
    private const string ExpectedPassword = "Quick123";

    public static bool TryLogin(string username, string password) =>
        username == ExpectedUsername && password == ExpectedPassword;

    public static bool Authenticate(int maxAttempts)
    {
        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            Console.Write("Username: ");
            string username = (Console.ReadLine() ?? "").Trim();

            Console.Write("Password: ");
            string password = Console.ReadLine() ?? "";

            if (TryLogin(username, password))
            {
                Console.WriteLine("Login successful.");
                return true;
            }

            Console.WriteLine($"Incorrect username or password. Attempts left: {maxAttempts - attempt}");
        }

        return false;
    }
}
