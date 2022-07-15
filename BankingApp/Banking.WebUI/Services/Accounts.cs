namespace Banking.WebUI.Services
{
    public class Account
    {
        public int Index { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
    }

    public class AccountTransfer
    {
        public int FromIndex { get; set; }

        public int ToIndex { get; set; }

        public decimal Amount { get; set; }
    }
}
