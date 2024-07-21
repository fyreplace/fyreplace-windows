namespace Fyreplace.Helpers
{
    public static class Users
    {
        public static bool IsUsernameValid(string username) => !string.IsNullOrWhiteSpace(username)
            && username.Length >= 3
            && username.Length <= 50;

        public static bool IsEmailValid(string email) => !string.IsNullOrWhiteSpace(email)
            && email.Length >= 3
            && email.Length <= 254
            && email.Contains('@');
    }
}
