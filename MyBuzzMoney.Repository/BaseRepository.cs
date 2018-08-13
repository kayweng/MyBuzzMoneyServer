using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using AWSSimpleClients.Clients;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyBuzzMoney.Repository
{
    public class BaseRepository<T>
    {
        #region Properties
        protected IDynamoDBContext DDBContext { get; set; }
        protected string DynamoTableName { get; set; }

        RegionEndpoint _region { get; set; }
        string _accessKey { get; set; }
        string _secretKey { get; set; }
        #endregion

        protected BaseRepository(string tableName)
        {
            _region = RegionEndpoint.GetBySystemName(Environment.GetEnvironmentVariable("region"));
            _accessKey = Environment.GetEnvironmentVariable("accessKey");
            _secretKey = Environment.GetEnvironmentVariable("secretKey");
            DynamoTableName = tableName;

            AWS.LoadAWSBasicCredentials(_region, _accessKey, _secretKey);

            InitRepository();
        }

        protected void InitRepository()
        {
            var config = new DynamoDBContextConfig { Conversion = DynamoDBEntryConversion.V2 };

            AWSConfigsDynamoDB.Context.TypeMappings[typeof(T)] = new Amazon.Util.TypeMapping(typeof(T), DynamoTableName);

            DDBContext = new DynamoDBContext(new AmazonDynamoDBClient(), config);
        }
    }
}
