using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Wasabi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private static IAmazonS3 _s3Client;
        private const string _bucketName = "myDirectoryName";
        private const string _objectName1 = "myFileName.txt";

        [HttpPost("Config")]
        public async Task<bool> Config()
        {
            // this is necessary for the endpoint
            var config = new AmazonS3Config { ServiceURL = "https://s3.wasabisys.com" };
            try
            {
                // create s3 connection with credential files and config.
                _s3Client = new AmazonS3Client("<AccessKey>", "<SecretKey>", config);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [HttpPost("Created_Bucket")]
        public async Task<string> CreateBucketAsync(string yourBucketName)
        {
            // Set Up connection
            bool config = await Config();
            if (!config)
                return "Configuration Failed!";

            try
            {
                var putBucketRequest = new PutBucketRequest
                {
                    BucketName = yourBucketName
                };
                await _s3Client.PutBucketAsync(putBucketRequest);
            }
            catch (AmazonS3Exception e)
            {
                return "Failed Creating Bucket! " + e.Message;
            }
            finally
            {
                //Close the Connection
                _s3Client.Dispose();
            }
            return "New Bucket Created";
        }

        [HttpPost("Upload_File_In_Bucket")]
        public async Task<string> UploadFileAsync(IFormFile file)
        {
            // Set Up connection
            bool config = await Config();
            if (!config)
                return "Configuration Failed!";

            try
            {
                var newMemoryStream = new MemoryStream();
                file.CopyTo(newMemoryStream);

                var putRequest = new PutObjectRequest
                {
                    InputStream=newMemoryStream,
                    BucketName = _bucketName, // Name your directory already created in Wasabi Cloud Storage.
                    Key = _objectName1, // Your file will be saved with this name.
                    CannedACL = S3CannedACL.PublicRead
                };
                putRequest.Metadata.Add("x-amz-meta-title", "SomeTitle");
                await _s3Client.PutObjectAsync(putRequest);
            }
            catch (AmazonS3Exception e)
            {
                return "File Not Upoaded!" + e.Message;
            }
            finally
            {
                _s3Client.Dispose();
            }
            return "File Upoaded To Wasabi Cloud!";
        }

        [HttpPost("Upload_File_In_Bucket_using_file_path")]
        public async Task<string> UploadFileUsingPathAsync()
        {
            // Set Up connection
            bool config = await Config();
            if (!config)
                return "Configuration Failed!";

            try
            {
                // Give the path of your file here that includes the file name & file extension.
                var filePath = $"C:/Users/userName/Desktop/fileName.txt";

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
                return "File Not Upoaded!" + e.Message;
            }
            finally
            {
                _s3Client.Dispose();
            }
            return "File Upoaded To Wasabi Cloud!";
        }

        [HttpGet("Get_File_From_Bucket")]
        public async Task<string> GetFileAsync()
        {
            // Set Up connection
            bool config = await Config();
            if (!config)
                return "File Not Upoaded!";

            try
            {
                // create a get object request
                var getObjectRequest = new GetObjectRequest()
                {
                    BucketName = _bucketName,
                    Key = _objectName1
                };

                TransferUtility transferUtility = new TransferUtility(_s3Client);
                await transferUtility.DownloadAsync("C:/Users/userName/Desktop\\" + _objectName1, _bucketName, _objectName1);
            }
            catch (AmazonS3Exception e)
            {
                return "Error :" + e.Message;
            }
            finally
            {
                _s3Client.Dispose();
            }
            return "File Downloaded!";
        }

        [HttpDelete("Delete_File_In_Bucket")]
        public async Task<string> DeleteFileAsync(string yourBucketName, string yourFileName_withExtension)
        {
            // Set Up connection
            bool config = await Config();
            if (!config)
                return "Configuration Failed!";

            try
            {
                // create a delete object request
                var deleteObjectRequest = new DeleteObjectRequest() 
                { 
                    BucketName = yourBucketName, 
                    Key = yourFileName_withExtension 
                };
                // call the delete operation
                await _s3Client.DeleteObjectAsync(deleteObjectRequest);
            }
            catch (AmazonS3Exception e)
            {
                return "Failed Deleting File! " + e.Message;
            }
            finally
            {
                //Close the Connection
                _s3Client.Dispose();
            }
            return "File Deleted!";
        }
    }
}
