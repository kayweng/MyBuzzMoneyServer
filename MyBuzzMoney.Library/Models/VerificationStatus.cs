
using Newtonsoft.Json;

namespace MyBuzzMoney.Library.Models
{
    public class VerificationStatus
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("emailProcess")]
        public VerificationProcess EmailProcess { get; set; }

        [JsonProperty("identityProcess")]
        public VerificationProcess IdentityProcess { get; set; }

        [JsonProperty("mobileProcess")]
        public VerificationProcess MobileProcess { get; set; }

        [JsonProperty("addressProcess")]
        public VerificationProcess AddressProcess { get; set; }

        [JsonProperty("createdOn")]
        public string CreatedOn { get; set; }

        [JsonProperty("modifiedOn")]
        public string ModifiedOn { get; set; }
    }
}
