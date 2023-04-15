using Abstractions.Interfaces;
using Amazon.S3.Model;
using Amazon.S3;
using Amazon;
using Amazon.S3.Util;
using Microsoft.Extensions.Configuration;
using Amazon.Runtime;
using System.Net;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;

namespace Services
{
    public class AwsS3StorageService: IStorageService
    {
        private readonly IConfiguration _configuration;
        private const string bucketName = "csharp-examples-bucket";
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast1;
        private IAmazonS3 s3Client;

        public AwsS3StorageService(IConfiguration configuration)
        {
            _configuration = configuration;
            s3Client = new AmazonS3Client(bucketRegion);
            CreateBucketAsync().Wait();
        }

        public async Task<string> Upload(Stream stream)
        {
            // Create a unique S3 key name
            var fileName = Guid.NewGuid().ToString() + ".pdf";

            // Upload the file to S3
            await s3Client.PutObjectAsync(new PutObjectRequest()
            {
                InputStream = stream,
                BucketName = bucketName,
                Key = fileName,
            });


            // Generate a presigned url pointing to our new file
            var url = s3Client.GetPreSignedURL(new GetPreSignedUrlRequest()
            {
                BucketName = bucketName,
                Key = fileName,
                Expires = DateTime.UtcNow.AddMinutes(10)
            });

            return url;
        }

        async Task<string> CreateBucketAsync()
        {
            string bucketLocation="";
            try
            {
                if (!await AmazonS3Util.DoesS3BucketExistV2Async(s3Client, bucketName))
                {
                    var putBucketRequest = new PutBucketRequest
                    {
                        BucketName = bucketName,
                        UseClientRegion = true
                    };

                    PutBucketResponse putBucketResponse = await s3Client.PutBucketAsync(putBucketRequest);
                }
                // Retrieve the bucket location.
                bucketLocation = await FindBucketLocationAsync(s3Client);
                
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            return bucketLocation;
        }

        async Task<string> FindBucketLocationAsync(IAmazonS3 client)
        {
            string bucketLocation;
            var request = new GetBucketLocationRequest()
            {
                BucketName = bucketName
            };
            GetBucketLocationResponse response = await client.GetBucketLocationAsync(request);
            bucketLocation = response.Location.ToString();
            return bucketLocation;
        }

    }
}