namespace BankApplication.Requests
{
    public static class Constants
    {
        public const string WELCOME_MSG = "Welcome to AwesomeGIC Bank!";

        public const string ASTERIK = "*******************************";

        public const string HELP_MSG = "What would you like to do?";

        public const string HELP_MSG1 = "Is there anything else you'd like to do?";

        public const string INPUT_TRANSACTION = "[I]nput Transactions";

        public const string DEFINE_INTEREST_RULES = "[D]efine Interest Rules";

        public const string PRINT_STATEMENT = "[P]rint Statement";

        public const string QUIT = "[Q]uit";

        public const string TRANSACTION_FORMAT = "Please enter transaction details in <Date>|<Account>|<Type>|<Amount> (or enter blank to go back to main menu):)";

        public const string DEFINE_RULE_FORMAT = "Please enter interest rules details in <Date>|<RuleId>|<Rate in %> (or enter blank to go back to main menu):)";

        public const string PRINT_FORMAT = "Please enter account and month to generate the statement in <Account>|<Month> (or enter blank to go back to main menu):)";

        public const string ACCOUNT_TYPE_W = "W";

        public const string ACCOUNT_TYPE_D = "D";

        public const string INPUT = "I";

        public const string RULES = "D";

        public const string PRINT = "P";

        public const string QUIT_PROCESS = "Q";

        public const string INTEREST_TYPE = "I";
    }

    public static class MenuOptions
    {
        public const string IT = "I";

        public const string DR = "D";

        public const string PS = "P";

        public const string QT = "Q";
    }
}
