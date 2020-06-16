using Amazon.Runtime.Internal.Util;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Jiggswap.Application.Images
{
    public class S3Image
    {
        public string Filename { get; set; }

        public string FileUrl { get; set; }
    }

    public interface IS3ImageHandler
    {
        public Task<S3Image> SaveImageToS3(byte[] imageData);

        public Task<bool> RemoveImageFromS3(string s3Filename);
    }

    public class S3ImageHandler : IS3ImageHandler
    {
        private readonly ILogger<S3ImageHandler> _logger;

        private readonly AmazonS3Client _s3Client;

        private readonly string _s3CdnBaseUrl;
        private readonly string _s3ImageBasePath;

        public S3ImageHandler(IConfiguration config, ILogger<S3ImageHandler> logger)
        {
            _logger = logger;

            _s3Client = new AmazonS3Client(config["AWS:AccessKeyId"], config["AWS:SecretKey"], Amazon.RegionEndpoint.USEast2);
            _s3CdnBaseUrl = config["AWS:CdnBaseUrl"];
            _s3ImageBasePath = config["AWS:ImageBaseDirectory"];
        }

        public async Task<S3Image> SaveImageToS3(byte[] imageData)
        {
            var s3TransferUtility = new TransferUtility(_s3Client);

            var s3FileName = _s3ImageBasePath + "/" + Path.GetRandomFileName().Replace(".", "") + ".png";

            using (var stream = new MemoryStream(imageData))
            {
                await s3TransferUtility.UploadAsync(stream, "jiggswap", s3FileName);
            }

            return new S3Image
            {
                Filename = s3FileName,
                FileUrl = _s3CdnBaseUrl + "/" + s3FileName
            };
        }

        public async Task<bool> RemoveImageFromS3(string s3Filename)
        {
            try
            {
                await _s3Client.DeleteObjectAsync(new DeleteObjectRequest
                {
                    BucketName = "jiggswap",
                    Key = s3Filename
                });
            }
            catch (AmazonS3Exception e)
            {
                _logger.LogError(e, "Something went wrong deleting S3 image.");
                return false;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong deleting the S3 image.");
                return false;
            }

            return true;
        }
    }
}