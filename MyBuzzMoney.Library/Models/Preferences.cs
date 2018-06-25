using MyBuzzMoney.Library.Enums;
using Newtonsoft.Json;

namespace MyBuzzMoney.Library.Models
{
    public class Preferences
    {
        [JsonProperty("localCurrency")]
        public string LocalCurrency { get; set; }

        [JsonProperty("location")]
        public Location Location { get; set; }

        [JsonProperty("notifications")]
        public Notifications Notifications { get; set; }
    }
}
