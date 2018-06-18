using System;
using System.Collections.Generic;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using AWSSimpleClients.Clients;
using MyBuzzMoney.Library.Enums;
using MyBuzzMoney.Logging;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace MyBuzzMoney.UserPostConfirmation
{
    public class Function
    {
        #region Properties
        const string ConfirmSignUp = "PostConfirmation_ConfirmSignUp";
        const string EMPTY_STRING = "-";
        private string _tableName {get; set; }

        private RegionEndpoint _region { get; set; }
        private string _accessKey { get; set; }
        private string _secretKey { get; set; }
        
        #endregion

        #region Constructor
        public Function()
        {
            _region = RegionEndpoint.GetBySystemName(Environment.GetEnvironmentVariable("region"));
            _accessKey = Environment.GetEnvironmentVariable("accessKey");
            _secretKey = Environment.GetEnvironmentVariable("secretKey");
            _tableName = Environment.GetEnvironmentVariable("tableName");
        }

        public Function(RegionEndpoint region, string accessKey, string secretKey)
        {
            _region = region;
            _accessKey = accessKey;
            _secretKey = secretKey;
        }
        #endregion

        #region Function
        /// <summary>
        /// Create an user record to database when user confirmed the email verification.
        /// </summary>
        /// <param name="context"></param>
        public CognitoContext FunctionHandler (CognitoContext model, ILambdaContext context)
        {
            AWS.LoadAWSBasicCredentials(_region, _accessKey, _secretKey);

            Console.WriteLine(JsonConvert.SerializeObject(model));

            if (model.TriggerSource == ConfirmSignUp)
            {
                try
                {
                    UserAttributes attributes = model.Request.UserAttributes;
                    Dictionary<string, AttributeValue> userAttributes = new Dictionary<string, AttributeValue>
                    {
                        ["Email"] = new AttributeValue() { S = attributes.CognitoEmail_Alias },
                        ["EmailVerified"] = new AttributeValue() { BOOL = attributes.CognitoUser_Status == "CONFIRMED" ? true : false },
                        ["FirstName"] = new AttributeValue() { S = attributes.Name.Split(' ')[0] },
                        ["LastName"] = new AttributeValue() { S = attributes.Name.Split(' ')[1] },
                        ["Mobile"] = new AttributeValue() { S = attributes.Phone_Number == null ? EMPTY_STRING : attributes.Phone_Number },
                        ["MobileVerified"] = new AttributeValue() { BOOL = attributes.Phone_Number_Verified == "true" ? true : false },
                        ["Birthdate"] = new AttributeValue() { S = attributes.Birthdate.ToString() },
                        ["Gender"] = new AttributeValue() { S = EMPTY_STRING },
                        ["Address"] = new AttributeValue() { S = EMPTY_STRING },
                        ["AddressVerified"] = new AttributeValue() { BOOL = false },
                        ["Country"] = new AttributeValue() { S = EMPTY_STRING },
                        ["UserType"] = new AttributeValue() { S = UserType.Confirmed.ToString() },
                        ["ImageUrl"] = new AttributeValue() { S = EMPTY_STRING },
                        ["Active"] = new AttributeValue() { BOOL = true },
                        ["CreatedOn"] = new AttributeValue() { S = DateTime.UtcNow.ToLongDateString() },
                        ["ModifiedOn"] = new AttributeValue() { S = DateTime.UtcNow.ToLongDateString() }
                    };

                    var response = AWS.DynamoDB.PutItemAsync(new PutItemRequest()
                    {
                        TableName = _tableName,
                        Item = userAttributes
                    }).GetAwaiter().GetResult();

                    if (response.HttpStatusCode != System.Net.HttpStatusCode.OK && response.HttpStatusCode != System.Net.HttpStatusCode.Accepted)
                    {
                        throw new Exception(string.Format("Failed to create a confirmed user - {0}", userAttributes["Email"].S.ToString()));
                    }
                }
                catch (AmazonDynamoDBException exception)
                {
                    Logger.Instance.LogException(exception);
                }
                catch (Exception exception)
                {
                    Logger.Instance.LogException(exception);
                }
            }

            return model;
        }
        #endregion
    }
}
