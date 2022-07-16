using System.Text.Json.Serialization;

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
        [JsonPropertyName("fromIndex")]
        public int FromIndex { get; set; }

        [JsonPropertyName("toIndex")]
        public int ToIndex { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
    }
}
