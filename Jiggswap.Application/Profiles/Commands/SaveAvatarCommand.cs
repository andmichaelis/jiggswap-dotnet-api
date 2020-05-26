using Dapper;
using ImageMagick;
using Jiggswap.Application.Common;
using Jiggswap.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Jiggswap.Application.Profiles.Commands
{
    public class SaveAvatarCommand : IRequest<int>
    {
        public IFormFile ImageBlob { get; set; }

        public int ProfileId { get; set; }
    }

    public class SaveAvatarCommandHandler : IRequestHandler<SaveAvatarCommand, int>
    {
        private readonly IJiggswapDb _db;

        public SaveAvatarCommandHandler(IJiggswapDb db)
        {
            _db = db;
        }

        private async Task<byte[]> GetImageDataFromBlob(IFormFile blob)
        {
            using var stream = new MemoryStream();

            await blob.CopyToAsync(stream).ConfigureAwait(false);

            return stream.ToArray();
        }

        private byte[] ShrinkImage(byte[] imageData)
        {
            using var image = new MagickImage(imageData);

            image.Resize(256, 256);

            return image.ToByteArray();
        }

        public async Task<int> Handle(SaveAvatarCommand request, CancellationToken cancellationToken)
        {
            var imageData = await GetImageDataFromBlob(request.ImageBlob);

            imageData = ShrinkImage(imageData);

            using var conn = _db.GetConnection();

            var newImageId = await conn.QuerySingleAsync<int>("insert into images (image_data) values (@ImageData) returning id",
                new
                {
                    ImageData = imageData
                });

            var oldImageId = await conn.QuerySingleOrDefaultAsync<int?>(
                "select image_id from user_profiles where id = @ProfileId", new
                {
                    request.ProfileId
                });

            await conn.ExecuteAsync("update user_profiles set image_id = @ImageId where id = @ProfileId",
                new
                {
                    ImageId = newImageId,
                    request.ProfileId
                });

            if (oldImageId != null)
            {
                await conn.ExecuteAsync("delete from images where id = @OldImageId",
                    new
                    {
                        OldImageId = oldImageId
                    });
            }

            return newImageId;
        }
    }
}