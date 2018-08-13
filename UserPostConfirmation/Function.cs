using System;
using System.Collections.Generic;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using AWSSimpleClients.Clients;
using MyBuzzMoney.Library.Enums;
using MyBuzzMoney.Library.Models;
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
        private string _userTableName {get; set; }
        private string _settingTableName { get; set; }
        private string _verificationTableName { get; set; }
        private string _inboxTableName { get; set; }

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
            _userTableName = Environment.GetEnvironmentVariable("userTable");
            _settingTableName = Environment.GetEnvironmentVariable("settingTable");
            _inboxTableName = Environment.GetEnvironmentVariable("inboxTable");
            _verificationTableName = Environment.GetEnvironmentVariable("verificationTable");
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
                CreateUserProfile(model);

                CreateUserSetting(model);

                CreateVerificationStatus(model);

                var welcomeMessage = new Message()
                {
                    Email = model.Request.UserAttributes.Email,
                    Title = string.Format("Welcome, {0}, ", model.Request.UserAttributes.Name),
                    BodyContent = "Welcome, thank you for signing up my buzz money again. Your email is verified, Please complete the verification processes to be genuine user.",
                    MessageType = MessageType.Info.ToString(),
                    IsView = false,
                    Active = true,
                    CreatedOn = DateTime.UtcNow.ToLongDateString(),
                    ModifiedOn = DateTime.UtcNow.ToLongDateString()
                };

                SendMessage(welcomeMessage);
            }

            return model;
        }

        private void CreateUserProfile(CognitoContext model)
        {
            try
            {
                UserAttributes attributes = model.Request.UserAttributes;
                Dictionary<string, AttributeValue> userAttributes = new Dictionary<string, AttributeValue>
                {
                    ["Email"] = new AttributeValue() { S = attributes.CognitoEmail_Alias },
                    ["FirstName"] = new AttributeValue() { S = attributes.Name.Split(' ')[0] },
                    ["LastName"] = new AttributeValue() { S = attributes.Name.Split(' ')[1] },
                    ["Mobile"] = new AttributeValue() { S = attributes.Phone_Number == null ? EMPTY_STRING : attributes.Phone_Number },
                    ["Birthdate"] = new AttributeValue() { S = attributes.Birthdate.ToString() },
                    ["Gender"] = new AttributeValue() { S = EMPTY_STRING },
                    ["Address"] = new AttributeValue() { S = EMPTY_STRING },
                    ["Country"] = new AttributeValue() { S = EMPTY_STRING },
                    ["UserType"] = new AttributeValue() { S = UserType.Confirmed.ToString() },
                    ["ImageUrl"] = new AttributeValue() { S = EMPTY_STRING },
                    ["Active"] = new AttributeValue() { BOOL = true },
                    ["CreatedOn"] = new AttributeValue() { S = DateTime.UtcNow.ToLongDateString() },
                    ["ModifiedOn"] = new AttributeValue() { S = DateTime.UtcNow.ToLongDateString() }
                };

                var response = AWS.DynamoDB.PutItemAsync(new PutItemRequest()
                {
                    TableName = _userTableName,
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

        private void CreateUserSetting(CognitoContext model)
        {
            try
            {
                UserAttributes attributes = model.Request.UserAttributes;

                Preferences preferences = new Preferences
                {
                    LocalCurrency = "-",
                    Location = new Location()
                    {
                        Country = "-",
                        State = "-",
                        City = "-"
                    },
                    Notifications = new Notifications()
                    {
                        Expired = false,
                        Accepted = true,
                        Denied = false
                    }
                };

                Verifications verifications = new Verifications()
                {
                    EmailVerified = attributes.CognitoUser_Status == "CONFIRMED",
                    AddressVerified = false,
                    IdentityVerified = false,
                    MobileVerified = attributes.Phone_Number_Verified == "true",
                };

                
                Dictionary<string, AttributeValue> settingAttributes = new Dictionary<string, AttributeValue>
                {
                    ["Email"] = new AttributeValue() { S = attributes.CognitoEmail_Alias },
                    ["Preferences"] = new AttributeValue() { S = JsonConvert.SerializeObject(preferences) },
                    ["Verifications"] = new AttributeValue() { S = JsonConvert.SerializeObject(verifications) },
                    ["Active"] = new AttributeValue() { BOOL = true },
                    ["CreatedOn"] = new AttributeValue() { S = DateTime.UtcNow.ToLongDateString() },
                    ["ModifiedOn"] = new AttributeValue() { S = DateTime.UtcNow.ToLongDateString() }
                };

                var response = AWS.DynamoDB.PutItemAsync(new PutItemRequest()
                {
                    TableName = _settingTableName,
                    Item = settingAttributes
                }).GetAwaiter().GetResult();

                if (response.HttpStatusCode != System.Net.HttpStatusCode.OK && response.HttpStatusCode != System.Net.HttpStatusCode.Accepted)
                {
                    throw new Exception(string.Format("Failed to create an user setting - {0}", settingAttributes["Email"].S.ToString()));
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

        private void CreateVerificationStatus(CognitoContext model)
        {
            try
            {
                UserAttributes attributes = model.Request.UserAttributes;

                #region verification entity
                VerificationStatus verification = new VerificationStatus
                {
                    Email = attributes.CognitoEmail_Alias,
                    EmailProcess = new VerificationProcess
                    {
                        Status = VerifiedType.Verified.ToString(),
                        Reason = "Verified via confirmation email.",
                        Remark = "-",
                        Approver = "User",
                        ProcessDateTime = DateTime.UtcNow.ToLongDateString(),
                        Active = true
                    },
                    IdentityProcess = new VerificationProcess
                    {
                        Status = VerifiedType.Pending.ToString(),
                        Reason = "-",
                        Remark = "-",
                        Approver = "-",
                        ProcessDateTime = "-",
                        Active = true
                    },
                    MobileProcess = new VerificationProcess
                    {
                        Status = VerifiedType.Pending.ToString(),
                        Reason = "-",
                        Remark = "-",
                        Approver = "-",
                        ProcessDateTime = "-",
                        Active = true
                    },
                    AddressProcess = new VerificationProcess
                    {
                        Status = VerifiedType.Pending.ToString(),
                        Reason = "-",
                        Remark = "-",
                        Approver = "-",
                        ProcessDateTime = "-",
                        Active = true
                    },
                };
                #endregion

                Dictionary<string, AttributeValue> verificationAttribute = new Dictionary<string, AttributeValue>
                {
                    ["Email"] = new AttributeValue() { S = attributes.CognitoEmail_Alias },
                    ["EmailProcess"] = new AttributeValue() { S = JsonConvert.SerializeObject(verification.EmailProcess) },
                    ["IdentityProcess"] = new AttributeValue() { S = JsonConvert.SerializeObject(verification.IdentityProcess) },
                    ["MobileProcess"] = new AttributeValue() { S = JsonConvert.SerializeObject(verification.MobileProcess) },
                    ["AddressProcess"] = new AttributeValue() { S = JsonConvert.SerializeObject(verification.AddressProcess) },
                    ["CreatedOn"] = new AttributeValue() { S = DateTime.UtcNow.ToLongDateString() },
                    ["ModifiedOn"] = new AttributeValue() { S = DateTime.UtcNow.ToLongDateString() }
                };

                var response = AWS.DynamoDB.PutItemAsync(new PutItemRequest()
                {
                    TableName = _verificationTableName,
                    Item = verificationAttribute
                }).GetAwaiter().GetResult();

                if (response.HttpStatusCode != System.Net.HttpStatusCode.OK && response.HttpStatusCode != System.Net.HttpStatusCode.Accepted)
                {
                    throw new Exception(string.Format("Failed to create user verification processes - {0}", verificationAttribute["Email"].S.ToString()));
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

        private void SendMessage(Message message)
        {
            try
            {
                Dictionary<string, AttributeValue> messageAttribute = new Dictionary<string, AttributeValue>
                {
                    ["Email"] = new AttributeValue() { S = message.Email },
                    ["Title"] = new AttributeValue() { S = message.Title },
                    ["BodyContent"] = new AttributeValue() { S = message.BodyContent },
                    ["MessageType"] = new AttributeValue() { S = message.MessageType },
                    ["IsView"] = new AttributeValue() { BOOL = message.IsView },
                    ["Active"] = new AttributeValue() { BOOL = true },
                    ["CreatedOn"] = new AttributeValue() { S = DateTime.UtcNow.ToLongDateString() },
                    ["ModifiedOn"] = new AttributeValue() { S = DateTime.UtcNow.ToLongDateString() }
                };

                var response = AWS.DynamoDB.PutItemAsync(new PutItemRequest()
                {
                    TableName = _inboxTableName,
                    Item = messageAttribute
                }).GetAwaiter().GetResult();

                if (response.HttpStatusCode != System.Net.HttpStatusCode.OK && response.HttpStatusCode != System.Net.HttpStatusCode.Accepted)
                {
                    throw new Exception(string.Format("Failed to create inbox message - {0}", messageAttribute["Email"].S.ToString()));
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
        #endregion
    }
}
