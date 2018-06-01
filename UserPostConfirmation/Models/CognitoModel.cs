using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyBuzzMoney.UserPostConfirmation
{
    public class CognitoContext
    {
        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("userPoolId")]
        public string UserPoolId { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("callerContext")]
        public CallerContext Caller { get; set; }

        [JsonProperty("triggerSource")]
        public string TriggerSource { get; set; }

        [JsonProperty("request")]
        public PostConfirmationRequest Request { get; set; }

        [JsonProperty("response")]
        public PostConfirmationResponse Response { get; set; }
    }

    public class CallerContext
    {
        [JsonProperty("awsSdkVersion")]
        public string AwsSdkVersion { get; set; }

        [JsonProperty("clientId")]
        public string ClientId { get; set; }
    }

    public class PostConfirmationRequest
    {
        [JsonProperty("userAttributes")]
        public UserAttributes UserAttributes { get; set; }
    }

    public class PostConfirmationResponse
    {

    }

    [JsonObject(MemberSerialization.OptIn)]
    public class UserAttributes
    {
        [JsonProperty("sub")]
        public string Sub { get; set; }

        [JsonProperty("cognito:email_alias")]
        public string CognitoEmail_Alias { get; set; }

        [JsonProperty("cognito:user_status")]
        public string CognitoUser_Status { get; set; }

        [JsonProperty("birthdate")]
        public string Birthdate { get; set; }

        [JsonProperty("email_verified")]
        public string Email_Verified { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("phone_number_verified")]
        public string Phone_Number_Verified { get; set; }

        [JsonProperty("phone_number")]
        public string Phone_Number { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
    }
}
