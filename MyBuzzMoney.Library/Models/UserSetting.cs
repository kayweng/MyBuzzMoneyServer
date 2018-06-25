using MyBuzzMoney.Library.Enums;
using Newtonsoft.Json;

namespace MyBuzzMoney.Library.Models
{
    public class UserSetting
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("Preferences")]
        public Preferences Preferences { get; set; }

        [JsonProperty("LinkedAccounts")]
        public string LinkedAccounts { get; set; }

        [JsonProperty("Verifications")]
        public string Verifications { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("createdOn")]
        public string CreatedOn { get; set; }

        [JsonProperty("modifiedOn")]
        public string ModifiedOn { get; set; }
    }
}
