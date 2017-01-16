using Amazon.S3.Model;
using s3.example.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace s3.example.Services
{
    public interface IS3Service
    {
        Task<IEnumerable<string>> GetBucketObjectNamesAsync(string bucketName);
        Task<PutObjectResponse> CreateS3ObjectAsync(BucketObject bucketObject);
        Task<string> GetS3FileContentAsync(BucketObject bucketObject);
        Task RemoveS3ObjectAsync(BucketObject bucketObject);
        Task RemoveS3BucketAsync(string bucketName);
    }
}
