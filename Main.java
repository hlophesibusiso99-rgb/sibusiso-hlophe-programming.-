import java.util.Scanner;

/**
 * Console application entry point: registers a user, then logs them in.
 */
public class Main {

    public static void main(String[] args) {
        Scanner scanner = new Scanner(System.in);
        Login login = new Login();

        System.out.println("=== Chat App Registration ===");

        System.out.print("First name: ");
        String firstName = scanner.nextLine().trim();

        System.out.print("Last name: ");
        String lastName = scanner.nextLine().trim();

        while (!login.isRegistered()) {
            System.out.print("Username: ");
            String username = scanner.nextLine().trim();

            System.out.print("Password: ");
            String password = scanner.nextLine().trim();

            System.out.print("Cell phone number: ");
            String cellPhoneNumber = scanner.nextLine().trim();

            System.out.println(login.registerUser(firstName, lastName, username,
                                                  password, cellPhoneNumber));
            System.out.println();
        }

        System.out.println("=== Login ===");

        boolean loggedIn = false;
        while (!loggedIn) {
            System.out.print("Username: ");
            String loginUsername = scanner.nextLine().trim();

            System.out.print("Password: ");
            String loginPassword = scanner.nextLine().trim();

            loggedIn = login.loginUser(loginUsername, loginPassword);
            System.out.println(login.returnLoginStatus(loggedIn));
            System.out.println();
        }

        scanner.close();
    }
}
