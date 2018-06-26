using MyBuzzMoney.Library.Enums;
using Newtonsoft.Json;

namespace MyBuzzMoney.Library.Models
{
    public class UserSetting
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("preferences")]
        public string Preferences { get; set; }

        [JsonProperty("linkedAccounts")]
        public string LinkedAccounts { get; set; }

        [JsonProperty("verifications")]
        public string Verifications { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("createdOn")]
        public string CreatedOn { get; set; }

        [JsonProperty("modifiedOn")]
        public string ModifiedOn { get; set; }
    }
}
