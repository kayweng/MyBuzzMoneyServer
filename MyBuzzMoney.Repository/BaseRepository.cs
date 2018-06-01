using Amazon;
using AWSSimpleClients.Clients;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyBuzzMoney.Repository
{
    public class BaseRepository
    {
        #region Properties
        RegionEndpoint _region { get; set; }
        string _accessKey { get; set; }
        string _secretKey { get; set; }
        #endregion

        protected BaseRepository()
        {
            _region = RegionEndpoint.GetBySystemName(Environment.GetEnvironmentVariable("region"));
            _accessKey = Environment.GetEnvironmentVariable("accessKey");
            _secretKey = Environment.GetEnvironmentVariable("secretKey");

            AWS.LoadAWSBasicCredentials(_region, _accessKey, _secretKey);
        }
    }
}
