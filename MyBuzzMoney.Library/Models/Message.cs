using Newtonsoft.Json;

namespace MyBuzzMoney.Library.Models
{
    public class Message
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("bodyContent")]
        public string BodyContent { get; set; }

        [JsonProperty("messageType")]
        public string MessageType { get; set; }

        [JsonProperty("isView")]
        public bool IsView { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("createdOn")]
        public string CreatedOn { get; set; }

        [JsonProperty("modifiedOn")]
        public string ModifiedOn { get; set; }
    }
}
