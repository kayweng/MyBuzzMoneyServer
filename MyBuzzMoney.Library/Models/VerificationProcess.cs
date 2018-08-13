using Newtonsoft.Json;
using System;

namespace MyBuzzMoney.Library.Models
{
    public class VerificationProcess
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }

        [JsonProperty("remark")]
        public string Remark { get; set; }

        [JsonProperty("approver")]
        public string Approver { get; set; }

        [JsonProperty("processDateTime")]
        public string ProcessDateTime { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }
    }
}
