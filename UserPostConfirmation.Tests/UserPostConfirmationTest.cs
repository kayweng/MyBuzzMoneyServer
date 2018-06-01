using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;

using UserPostConfirmation;
using MyBuzzMoney.UserPostConfirmation;

using Newtonsoft.Json;

namespace UserPostConfirmation.Tests
{
    public class UserPostConfirmationTest
    {
        [Fact]
        public void FunctionTest()
        {
            string testData = "{\r\n\t\"version\": \"1\",\r\n\t\"region\": \"ap-southeast-1\",\r\n\t\"userPoolId\": \"ap-southeast-1_xNdr3g62r\",\r\n\t\"userName\": \"09621d65-0c1c-4ee0-9ab6-44f7d1aba8b4\",\r\n\t\"callerContext\": {\r\n\t\t\"awsSdkVersion\": \"aws-sdk-java-console\",\r\n\t\t\"clientId\": \"2gk449k1v21b6sci549b4klssp\"\r\n\t},\r\n\t\"triggerSource\": \"PostConfirmation_ConfirmSignUp\",\r\n\t\"request\": {\r\n\t\t\"userAttributes\": {\r\n\t\t\t\"sub\": \"09621d65-0c1c-4ee0-9ab6-44f7d1aba8b4\",\r\n\t\t\t\"cognito:email_alias\": \"unitest@gmail.com\",\r\n\t\t\t\"cognito:user_status\": \"CONFIRMED\",\r\n\t\t\t\"birthdate\": \"1986-07-19\",\r\n\t\t\t\"email_verified\": \"true\",\r\n\t\t\t\"name\": \"kay weng foong\",\r\n\t\t\t\"phone_number_verified\": \"false\",\r\n\t\t\t\"phone_number\": \"+60165490075\",\r\n\t\t\t\"email\": \"kaylek207@gmail.com\"\r\n\t\t}\r\n\t},\r\n\t\"response\":\r\n\t{\r\n\t\r\n\t}\r\n}";

            try
            {
                var function = new Function(Amazon.RegionEndpoint.APSoutheast1, "", "");
                var context = new TestLambdaContext();
                CognitoContext model = JsonConvert.DeserializeObject<CognitoContext>(testData);

                CognitoContext response = function.FunctionHandler(model, context);

                Assert.NotNull(response);
                Assert.True(response.Version == "1");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
