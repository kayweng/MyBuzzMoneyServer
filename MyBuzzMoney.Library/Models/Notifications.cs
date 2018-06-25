using MyBuzzMoney.Library.Enums;
using Newtonsoft.Json;

namespace MyBuzzMoney.Library.Models
{
    public class Notifications
    {
        [JsonProperty("expired")]
        public bool Expired { get; set; }

        [JsonProperty("accepted")]
        public bool Accepted { get; set; }

        [JsonProperty("denied")]
        public bool Denied { get; set; }
    }
}
