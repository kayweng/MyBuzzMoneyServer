using Amazon.DynamoDBv2.Model;
using MyBuzzMoney.Library.Models;
using MyBuzzMoney.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AWSSimpleClients.Clients;
using Newtonsoft.Json;

namespace MyBuzzMoney.Repository
{
    public class VerificationRepository : BaseRepository<VerificationStatus>, IVerificationRepository
    {
        public VerificationRepository(string tableName) : base(tableName)
        {

        }

        public async Task<VerificationStatus> RetrieveVerificationStatus(string username)
        {
            try
            {
                var verificationStatus = await DDBContext.LoadAsync<VerificationStatus>(username);

                return verificationStatus;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdateMobileVerification(string username, VerificationProcess process)
        {
            try
            {
                var request = new UpdateItemRequest
                {
                    TableName = DynamoTableName,
                    Key = new Dictionary<string, AttributeValue>()
                    {
                        { "Email", new AttributeValue { S = username } }
                    },
                    ExpressionAttributeNames = new Dictionary<string, string>()
                    {
                        {"#MobileProcess", "MobileProcess"},
                        {"#ModifiedOn", "ModifiedOn"}
                    },
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
                    {
                        {":process", new AttributeValue { S = JsonConvert.SerializeObject(process) }},
                        {":modifiedOn", new AttributeValue { S = DateTime.UtcNow.ToLongDateString() }}
                    },

                    UpdateExpression = "SET #MobileProcess = :process, #ModifiedOn = :modifiedOn"
                };

                return await AWS.DynamoDB.UpdateItemAsync(request).ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        Console.WriteLine(task.Exception.Message);
                    }

                    return !task.IsFaulted;
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
