/**
 * Registration and login for the chat app.
 */
public class Login {

    private final Validator validator = new Validator();
    private User registeredUser;

    public boolean checkUserName(String username) {
        return validator.checkUserName(username);
    }

    public boolean checkPasswordComplexity(String password) {
        return validator.checkPasswordComplexity(password);
    }

    public boolean checkCellPhoneNumber(String cellPhoneNumber) {
        return validator.checkCellPhoneNumber(cellPhoneNumber);
    }

    /**
     * Stores the user's details if every field is valid, and returns a
     * message describing the outcome.
     */
    public String registerUser(String firstName, String lastName, String username,
                               String password, String cellPhoneNumber) {

        if (!checkUserName(username)) {
            return "Username is not correctly formatted, please ensure that your "
                 + "username contains an underscore and is no more than five characters in length.";
        }

        if (!checkPasswordComplexity(password)) {
            return "Password is not correctly formatted, please ensure that the password "
                 + "contains at least eight characters, a capital letter, a number and a special character.";
        }

        if (!checkCellPhoneNumber(cellPhoneNumber)) {
            return "Cell phone number incorrectly formatted or does not contain "
                 + "international code.";
        }

        registeredUser = new User(firstName, lastName, username, password, cellPhoneNumber);

        return "Username successfully captured.\n"
             + "Password successfully captured.\n"
             + "Cell phone number successfully added.";
    }

    /**
     * True when the registration succeeded, i.e. there is an account to log in to.
     */
    public boolean isRegistered() {
        return registeredUser != null;
    }

    /**
     * Returns true when the supplied credentials match the registered ones.
     */
    public boolean loginUser(String username, String password) {
        return registeredUser != null
            && registeredUser.getUsername().equals(username)
            && registeredUser.getPassword().equals(password);
    }

    /**
     * Returns the message shown to the user after a login attempt.
     */
    public String returnLoginStatus(boolean loginSuccessful) {
        if (loginSuccessful) {
            return "Welcome " + registeredUser.getFirstName() + ", "
                 + registeredUser.getLastName() + " it is great to see you again.";
        }
        return "Username or password incorrect, please try again.";
    }
}
