using MyBuzzMoney.Library.Enums;
using Newtonsoft.Json;

namespace MyBuzzMoney.Library.Models
{
    public class Verifications
    {
        [JsonProperty("emailVerified")]
        public bool EmailVerified { get; set; }

        [JsonProperty("mobileVerified")]
        public bool MobileVerified { get; set; }

        [JsonProperty("addressVerified")]
        public bool AddressVerified { get; set; }

        [JsonProperty("identityVerified")]
        public bool IdentityVerified { get; set; }
    }
}
