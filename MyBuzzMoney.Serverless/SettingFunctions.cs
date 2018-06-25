using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon;
using AWSSimpleClients.Clients;
using Newtonsoft.Json;
using MyBuzzMoney.Repository;
using System.IO;
using Amazon.S3.Transfer;
using System.Text;
using MyBuzzMoney.Library.Helpers;
using MyBuzzMoney.Library.Models;

namespace MyBuzzMoney.Serverless
{
    public class SettingFunctions
    {
        #region Properties
        public string _tableName { get; set; }
        private RegionEndpoint _region { get; set; }
        private string _accessKey { get; set; }
        private string _secretKey { get; set; }

        private Dictionary<string, string> _responseHeader { get; set; }
        #endregion

        #region Constuctor
        public SettingFunctions()
        {
            _region = RegionEndpoint.GetBySystemName(Environment.GetEnvironmentVariable("region"));
            _accessKey = Environment.GetEnvironmentVariable("accessKey");
            _secretKey = Environment.GetEnvironmentVariable("secretKey");
            _tableName = Environment.GetEnvironmentVariable("tableName");
            _responseHeader = new Dictionary<string, string>() {
                { "Content-Type", "application/json" },
                { "Access-Control-Allow-Origin", "*" }
            };

            AWS.LoadAWSBasicCredentials(_region, _accessKey, _secretKey);
        }
        #endregion

        #region API Method
        public async Task<APIGatewayProxyResponse> GetUserSettingAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            string username = null;

            if (request.PathParameters.ContainsKey("username"))
            {
                username = request.PathParameters["username"].ToString();
            }

            if (!string.IsNullOrEmpty(username))
            {
                var repo = new SettingRepository(_tableName);
                var setting = await repo.RetrieveUserSetting(username);

                if (setting != null)
                {
                    return new APIGatewayProxyResponse
                    {
                        StatusCode = (int)HttpStatusCode.OK,
                        Body = JsonConvert.SerializeObject(setting),
                        Headers = _responseHeader
                    };
                }
            }

            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.NotFound
            };
        }

        public async Task<APIGatewayProxyResponse> PostUserPreferencesAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            string username = null;

            try
            {
                if (request.PathParameters.ContainsKey("username"))
                {
                    username = request.PathParameters["username"].ToString();
                }

                if (!string.IsNullOrEmpty(username))
                {
                    var repo = new SettingRepository(_tableName);
                    var setting = JsonConvert.DeserializeObject<UserSetting>(request.Body);

                    bool saved = await repo.UpdatePreferences(setting);

                    if (saved)
                    {
                        return new APIGatewayProxyResponse()
                        {
                            StatusCode = (int)HttpStatusCode.OK,
                            Body = JsonConvert.SerializeObject(setting),
                            Headers = _responseHeader
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogLine(ex.Message);

                return new APIGatewayProxyResponse
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Body = ex.Message
                };
            }

            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest
            };
        }

        public async Task<APIGatewayProxyResponse> PostUserLinkedAccountAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            string username = null;

            try
            {
                if (request.PathParameters.ContainsKey("username"))
                {
                    username = request.PathParameters["username"].ToString();
                }

                if (!string.IsNullOrEmpty(username))
                {
                    var repo = new SettingRepository(_tableName);
                    var setting = JsonConvert.DeserializeObject<UserSetting>(request.Body);

                    bool saved = await repo.UpdateLinkedAccount(setting);

                    if (saved)
                    {
                        return new APIGatewayProxyResponse()
                        {
                            StatusCode = (int)HttpStatusCode.OK,
                            Body = JsonConvert.SerializeObject(setting),
                            Headers = _responseHeader
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogLine(ex.Message);

                return new APIGatewayProxyResponse
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Body = ex.Message
                };
            }

            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest
            };
        }
        #endregion
    }
}
