using System;
using System.Collections.Generic;
using System.Text;

namespace MyBuzzMoney.Library.Helpers
{
    public static class BucketHelper
    {
        public static string GetS3BucketFilePath(string region, string bucketName, string fileName, string fileExtension)
        {
            string path = "https://s3-";

            path = path + region + "/";
            path = path + bucketName + "/";
            path = path + fileName + "." + fileExtension;

            return path;
        }
    }
}
