using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wasabi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private static IAmazonS3 _s3Client;
        private const string _bucketName = "directory1";
        private const string _objectName1 = "testFile.txt";

        [HttpPost("Config")]
        public async Task<bool> Config()
        {
            // 1. this is necessary for the endpoint
            var config = new AmazonS3Config { ServiceURL = "https://s3.wasabisys.com" };
            try
            {
                // create s3 connection with credential files and config.
                _s3Client = new AmazonS3Client("PMW2RNR6S4LGVSL893N0", "AD7ACmebdK0Bmqj337vpZh9loX2S3553M6GKMRJF", config);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [HttpPost("Upload_File_In_Bucket")]
        public async Task<string> UploadFileAsync()
        {
            // Set Up creadentials
            bool config = await Config();
            if (!config)
                return "File Not Upoaded!";

            try
            {
                // Give the path of your file here that includes the file name.
                var filePath = $"C:/Users/muhammad.sajid/OneDrive - MPARSEC LTD/Desktop/oop.txt";

                var putRequest = new PutObjectRequest
                {
                    BucketName = _bucketName, // Name your directory already created in Wasabi Cloud Storage.
                    Key = _objectName1, // Your file will be saved with this name.
                    FilePath = filePath
                };
                putRequest.Metadata.Add("x-amz-meta-title", "SomeTitle");
                await _s3Client.PutObjectAsync(putRequest);
            }
            catch (AmazonS3Exception e)
            {
                return e.Message;
            }
            return "OK";
        }

        [HttpPost("Get_File")]
        public async Task<string> GetFileAsync()
        {
            // Set Up creadentials
            bool config = await Config();
            if (!config)
                return "File Not Upoaded!";

            // create a get object request
            var getObjectRequest = new GetObjectRequest()
            {
                BucketName = _bucketName,
                Key = _objectName1
            };

            // call a get operation
            var res = await _s3Client.GetObjectAsync(getObjectRequest);
            return "OK";
        }
    }
}
