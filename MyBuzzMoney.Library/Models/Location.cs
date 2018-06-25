using MyBuzzMoney.Library.Enums;
using Newtonsoft.Json;

namespace MyBuzzMoney.Library.Models
{
    public class Location
    {
        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }
    }
}
