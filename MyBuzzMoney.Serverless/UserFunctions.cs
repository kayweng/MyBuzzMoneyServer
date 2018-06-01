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

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace MyBuzzMoney.Serverless
{
    public class UserFunctions
    {
        #region Properties
        public string _tableName { get; set; }
        private RegionEndpoint _region { get; set; }
        private string _accessKey { get; set; }
        private string _secretKey { get; set; }
        private string _imageBucketName { get; set; }

        private Dictionary<string, string> _responseHeader { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor that Lambda will invoke.
        /// </summary>
        public UserFunctions()
        {
            _region = RegionEndpoint.GetBySystemName(Environment.GetEnvironmentVariable("region"));
            _accessKey = Environment.GetEnvironmentVariable("accessKey");
            _secretKey = Environment.GetEnvironmentVariable("secretKey");
            _tableName = Environment.GetEnvironmentVariable("tableName");
            _imageBucketName = Environment.GetEnvironmentVariable("userImageBucket");
            _responseHeader = new Dictionary<string, string>() {
                { "Content-Type", "application/json" },
                { "Access-Control-Allow-Origin", "*" }
            };

            AWS.LoadAWSBasicCredentials(_region, _accessKey, _secretKey);
        }
        #endregion

        #region API Methods
        /// <summary>
        /// Retrieve buzz currency user async
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<APIGatewayProxyResponse> GetUserAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            string username = null;

            if (request.PathParameters.ContainsKey("username"))
            {
                username = request.PathParameters["username"].ToString();
            }

            if (!string.IsNullOrEmpty(username))
            {
                var repo = new UserRepository(_tableName);
                var user = await repo.RetrieveUser(username);

                if (user != null)
                {
                    return new APIGatewayProxyResponse
                    {
                        StatusCode = (int)HttpStatusCode.OK,
                        Body = JsonConvert.SerializeObject(user),
                        Headers = _responseHeader
                    };
                }
            }

            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.NotFound
            };
        }

        /// <summary>
        /// Save user profile record in dynamo db.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<APIGatewayProxyResponse> PostUserAsync(APIGatewayProxyRequest request, ILambdaContext context)
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
                    var repo = new UserRepository(_tableName);
                    var user = JsonConvert.DeserializeObject<UserProfile>(request.Body);

                    bool saved = await repo.UpdateUser(user);

                    if (saved)
                    {
                        var updatedUser = await repo.RetrieveUser(user.Email);

                        return new APIGatewayProxyResponse()
                        {
                            StatusCode = (int)HttpStatusCode.OK,
                            Body = JsonConvert.SerializeObject(updatedUser),
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

        /// <summary>
        /// Post upload user profile image to s3 bucket and return an image link for access.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public APIGatewayProxyResponse UploadProfileImage(APIGatewayProxyRequest request, ILambdaContext context)
        {
            TransferUtility utility = new TransferUtility(AWS.S3);
            string username = null;

            try
            {
                byte[] data = Convert.FromBase64String(request.Body);

                string decodedString = Encoding.UTF8.GetString(data);

                Dictionary<string, string> dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(decodedString);

                string dataString = dict["data"].ToString();

                if (request.PathParameters.ContainsKey("username") 
                    && !string.IsNullOrEmpty(dataString)
                    && dataString.Split(',').Length == 2)
                {
                    username = request.PathParameters["username"].ToString();

                    Byte[] imageBytes = Convert.FromBase64String(dataString.Split(',')[1]);

                    var file = new TransferUtilityUploadRequest()
                    {
                        InputStream = new MemoryStream(imageBytes),
                        BucketName = _imageBucketName,
                        Key = username + ".jpg",
                        ContentType = "image/jpeg",
                        CannedACL = Amazon.S3.S3CannedACL.AuthenticatedRead
                    };

                    utility.Upload(file);

                    var response = new APIGatewayProxyResponse
                    {
                        StatusCode = (int)HttpStatusCode.OK,
                        Body = file.FilePath,
                        Headers = _responseHeader
                    };

                    return response;
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
