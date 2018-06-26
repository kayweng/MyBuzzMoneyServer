using MyBuzzMoney.Library.Enums;
using Newtonsoft.Json;

namespace MyBuzzMoney.Library.Models
{
    public class LinkedAccount
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("bankName")]
        public string BankName { get; set; }

        [JsonProperty("swiftCode")]
        public string SwiftCode { get; set; }

        [JsonProperty("verified")]
        public string Verified { get; set; }

        [JsonProperty("createdOn")]
        public string CreatedOn { get; set; }
    }
}
