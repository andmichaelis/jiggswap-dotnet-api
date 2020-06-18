using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using Dapper;
using Google.Apis.Logging;
using Google.Apis.Util;
using ImageMagick;
using Jiggswap.Application.Common;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jiggswap.Application.Images.Commands
{
    public class SaveImageCommand : IRequest<int>
    {
        public IFormFile Image { get; set; }
        public int Width { get; }
        public int Height { get; }

        public SaveImageCommand(IFormFile image, int width, int height)
        {
            Image = image;
            Width = width;
            Height = height;
        }
    }

    public class SaveImageCommandHandler : IRequestHandler<SaveImageCommand, int>
    {
        private readonly ILogger<SaveImageCommandHandler> _logger;

        private readonly IJiggswapDb _db;
        private readonly IS3ImageHandler _s3ImageHandler;

        public SaveImageCommandHandler(ILogger<SaveImageCommandHandler> logger, IJiggswapDb db, IS3ImageHandler s3ImageHandler)
        {
            _logger = logger;
            _db = db;
            _s3ImageHandler = s3ImageHandler;
        }

        private async Task<byte[]> GetImageDataFromBlob(IFormFile blob)
        {
            using var stream = new MemoryStream();

            await blob.CopyToAsync(stream).ConfigureAwait(false);

            return stream.ToArray();
        }

        private byte[] ShrinkImage(byte[] imageData, int width, int height)
        {
            using var image = new MagickImage(imageData);
            image.Resize(width, height);

            return image.ToByteArray();
        }

        public async Task<int> Handle(SaveImageCommand request, CancellationToken cancellationToken)
        {
            using var conn = _db.GetConnection();

            var imageData = await GetImageDataFromBlob(request.Image);

            var shrunkImage = ShrinkImage(imageData, request.Width, request.Height);

            var s3Image = await _s3ImageHandler.SaveImageToS3(shrunkImage);

            return await conn.QuerySingleAsync<int>("insert into images(image_url, s3_filename) values (@S3Url, @S3FileName) returning id", new
            {
                S3Url = s3Image.FileUrl,
                S3FileName = s3Image.Filename
            });
        }
    }
}