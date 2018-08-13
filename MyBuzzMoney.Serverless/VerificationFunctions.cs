using System;
using System.Collections.Generic;
using System.Net;

using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon;
using AWSSimpleClients.Clients;
using Newtonsoft.Json;
using System.IO;
using Amazon.S3.Transfer;
using System.Threading.Tasks;
using MyBuzzMoney.Repository;
using MyBuzzMoney.Library.Models;

namespace MyBuzzMoney.Serverless
{
    public class VerificationFunctions
    {
        #region Properties
        public string _tableName { get; set; }
        private RegionEndpoint _region { get; set; }
        private string _accessKey { get; set; }
        private string _secretKey { get; set; }
        private string _identityDocumentBucketName { get; set; }
        private string _addressDocumentBucketName { get; set; }
        private string _approverEmail { get; set; }

        private Dictionary<string, string> _responseHeader { get; set; }
        #endregion

        #region Constuctor
        public VerificationFunctions()
        {
            _region = RegionEndpoint.GetBySystemName(Environment.GetEnvironmentVariable("region"));
            _accessKey = Environment.GetEnvironmentVariable("accessKey");
            _secretKey = Environment.GetEnvironmentVariable("secretKey");
            _tableName = Environment.GetEnvironmentVariable("tableName");
            _identityDocumentBucketName = Environment.GetEnvironmentVariable("identityDocumentBucketName");
            _addressDocumentBucketName = Environment.GetEnvironmentVariable("addressDocumentBucketName");

            _responseHeader = new Dictionary<string, string>() {
                { "Content-Type", "application/json" },
                { "Access-Control-Allow-Origin", "*" }
            };

            AWS.LoadAWSBasicCredentials(_region, _accessKey, _secretKey);
        }
        #endregion

        #region API Method
        public async Task<APIGatewayProxyResponse> GetVerificationAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            string username = null;

            if (request.PathParameters.ContainsKey("username"))
            {
                username = request.PathParameters["username"].ToString();
            }

            if (!string.IsNullOrEmpty(username))
            {
                var repo = new VerificationRepository(_tableName);
                var setting = await repo.RetrieveVerificationStatus(username);

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

        public APIGatewayProxyResponse PostIdentityDocument(APIGatewayProxyRequest request, ILambdaContext context)
        {
            TransferUtility utility = new TransferUtility(AWS.S3);
            string username = null;

            try
            {
                Dictionary<string, string> dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(request.Body);

                string dataString = dict["data"].ToString();

                if (request.PathParameters.ContainsKey("username")
                    && !string.IsNullOrEmpty(dataString)
                    && dataString.Split(',').Length == 2)
                {
                    username = request.PathParameters["username"].ToString();

                    var file = PostVerificationDocument(username, dataString, _identityDocumentBucketName, "jpg", "image/jpeg", Amazon.S3.S3CannedACL.AuthenticatedRead);

                    if (file != null)
                    {
                        // TODO: send email to approver user

                        var response = new APIGatewayProxyResponse
                        {
                            StatusCode = (int)HttpStatusCode.OK,
                            Body = file.FilePath,
                            Headers = _responseHeader
                        };

                        return response;
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

        public async Task<APIGatewayProxyResponse> PostMobileVerificationAsync(APIGatewayProxyRequest request, ILambdaContext context)
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
                    var repo = new VerificationRepository(_tableName);
                    var process = JsonConvert.DeserializeObject<VerificationProcess>(request.Body);

                    bool saved = await repo.UpdateMobileVerification(username, process);

                    if (saved)
                    {
                        return new APIGatewayProxyResponse()
                        {
                            StatusCode = (int)HttpStatusCode.OK,
                            Body = JsonConvert.SerializeObject(process),
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

        public APIGatewayProxyResponse PostAddressDocument(APIGatewayProxyRequest request, ILambdaContext context)
        {
            TransferUtility utility = new TransferUtility(AWS.S3);
            string username = null;

            try
            {
                Dictionary<string, string> dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(request.Body);

                string dataString = dict["data"].ToString();

                if (request.PathParameters.ContainsKey("username")
                    && !string.IsNullOrEmpty(dataString)
                    && dataString.Split(',').Length == 2)
                {
                    username = request.PathParameters["username"].ToString();

                    var file = PostVerificationDocument(username, dataString, _addressDocumentBucketName, "jpg", "image/jpeg", Amazon.S3.S3CannedACL.AuthenticatedRead);

                    if (file != null)
                    {
                        // TODO: send email to approver user

                        var response = new APIGatewayProxyResponse
                        {
                            StatusCode = (int)HttpStatusCode.OK,
                            Body = file.FilePath,
                            Headers = _responseHeader
                        };

                        return response;
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

        #region Private Method
        private TransferUtilityUploadRequest PostVerificationDocument(string username, string dataString, string bucketName, string imageType, string contentType, Amazon.S3.S3CannedACL acl)
        {
            TransferUtility utility = new TransferUtility(AWS.S3);

            Byte[] imageBytes = Convert.FromBase64String(dataString.Split(',')[1]);

            var file = new TransferUtilityUploadRequest()
            {
                InputStream = new MemoryStream(imageBytes),
                BucketName = bucketName,
                Key = username + "." + imageType,
                ContentType = contentType,
                CannedACL = acl
            };

            utility.Upload(file);

            return file;
        }
        #endregion
    }
}
