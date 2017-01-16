using Microsoft.AspNetCore.Mvc;
using s3.example.Models;
using s3.example.Services;
using System;
using System.Threading.Tasks;

namespace s3.example.Controllers
{
    [Route("api/[controller]")]
    public class S3Controller : Controller
    {
        private readonly IS3Service _s3Service;

        public S3Controller(IS3Service s3Service)
        {
            _s3Service = s3Service;
        }

        // GET api/s3
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hello");
        }

        // GET api/s3/bucketname
        [HttpGet("{bucketname}")]
        public async Task<IActionResult> GetBucketObjectsAsync(string bucketName)
        {
            try
            {
                var result = await _s3Service.GetBucketObjectNamesAsync(bucketName);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET api/s3/bucketname/key
        [HttpGet("{bucketname}/{key}")]
        public async Task<IActionResult> GetAsync([FromRoute] BucketObject bucketObject)
        {
            try
            {
                var content = await _s3Service.GetS3FileContentAsync(bucketObject);
                return Ok(content);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST api/s3
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] BucketObject bucketObject)
        {
            try
            {
                var response = await _s3Service.CreateS3ObjectAsync(bucketObject);
                return CreatedAtAction("GetAsync", "S3", bucketObject, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // DELETE api/s3/bucketname/key
        [HttpDelete("{bucketname}/{key}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] BucketObject bucketObject)
        {
            try
            {
                await _s3Service.RemoveS3ObjectAsync(bucketObject);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // DELETE api/s3/bucket/bucketname
        [HttpDelete("bucket/{bucketname}")]
        public async Task<IActionResult> DeleteBucketAsync(string bucketName)
        {
            try
            {
                await _s3Service.RemoveS3BucketAsync(bucketName);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
