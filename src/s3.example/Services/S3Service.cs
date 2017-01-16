using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using s3.example.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace s3.example.Services
{
    public class S3Service : IS3Service
    {
        private readonly IConfigurationRoot _configurationRoot;

        public S3Service(IConfigurationRoot configurationRoot)
        {
            _configurationRoot = configurationRoot;
        }

        private IAmazonS3 GetAmazonS3Client() => _configurationRoot.GetAWSOptions().CreateServiceClient<IAmazonS3>();

        public async Task<string> GetS3FileContentAsync(BucketObject bucketObject)
        {
            using (var client = GetAmazonS3Client())
            {
                var getObjectRequest = new GetObjectRequest()
                {
                    BucketName = bucketObject.BucketName,
                    Key = bucketObject.Key
                };

                using (var response = await client.GetObjectAsync(getObjectRequest))
                {
                    using (var streamReader = new StreamReader(response.ResponseStream))
                    {
                        return await streamReader.ReadToEndAsync();
                    }
                }
            }
        }

        public async Task RemoveS3ObjectAsync(BucketObject bucketObject)
        {
            using (var client = GetAmazonS3Client())
            {
                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = bucketObject.BucketName,
                    Key = bucketObject.Key
                };
                await client.DeleteObjectAsync(deleteObjectRequest);
            }
        }

        public async Task<PutObjectResponse> CreateS3ObjectAsync(BucketObject bucketObject)
        {
            using (var client = GetAmazonS3Client())
            {
                if (!await IsBucketExistsAsync(bucketObject.BucketName))
                    await CreateBucketAsync(bucketObject.BucketName);

                PutObjectRequest request = new PutObjectRequest
                {
                    BucketName = bucketObject.BucketName,
                    Key = bucketObject.Key,
                    ContentBody = "hello world from s3 bucket object"
                };
                return await client.PutObjectAsync(request);
            }
        }

        private async Task<bool> IsBucketExistsAsync(string bucketName)
        {
            using (var client = GetAmazonS3Client())
            {
                var result = await client.ListBucketsAsync();
                return result.Buckets.Any(x => x.BucketName.Equals(bucketName, StringComparison.CurrentCultureIgnoreCase));
            }
        }

        private async Task CreateBucketAsync(string bucketName)
        {
            using (var client = GetAmazonS3Client())
            {
                var putBucketRequest = new PutBucketRequest
                {
                    BucketName = bucketName
                };
                await client.PutBucketAsync(putBucketRequest);
            }
        }

        public async Task RemoveS3BucketAsync(string bucketName)
        {
            using (var client = GetAmazonS3Client())
            {
                var deleteBucketRequest = new DeleteBucketRequest()
                {
                    BucketName = bucketName
                };

                await client.DeleteBucketAsync(deleteBucketRequest);
            }
        }

        public async Task<IEnumerable<string>> GetBucketObjectNamesAsync(string bucketName)
        {
            using (var client = GetAmazonS3Client())
            {
                var listObjectsRequest = new ListObjectsRequest
                {
                    BucketName = bucketName
                };
                var result = await client.ListObjectsAsync(listObjectsRequest);
                return result.S3Objects.Select(x => x.Key);
            }
        }
    }
}
