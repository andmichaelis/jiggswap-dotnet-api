using Dapper;
using ImageMagick;
using Jiggswap.Application.Common;
using Jiggswap.Application.Common.Interfaces;
using Jiggswap.Application.Images;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Jiggswap.Application.Profiles.Commands
{
    public class SaveAvatarCommand : IRequest<int>
    {
        public int ImageId { get; set; }

        public int ProfileId { get; set; }
    }

    public class SaveAvatarCommandHandler : IRequestHandler<SaveAvatarCommand, int>
    {
        private readonly IJiggswapDb _db;
        private readonly IS3ImageHandler _s3ImageHandler;

        public SaveAvatarCommandHandler(IJiggswapDb db, IS3ImageHandler s3ImageHandler)
        {
            _db = db;

            _s3ImageHandler = s3ImageHandler;
        }

        public async Task<int> Handle(SaveAvatarCommand request, CancellationToken cancellationToken)
        {
            using var conn = _db.GetConnection();

            var oldImageId = await conn.QuerySingleOrDefaultAsync<int?>(
                "select image_id from user_profiles where id = @ProfileId", new
                {
                    request.ProfileId
                });

            await conn.ExecuteAsync("update user_profiles set image_id = @ImageId where id = @ProfileId",
                new
                {
                    request.ImageId,
                    request.ProfileId
                });

            if (oldImageId != null)
            {
                await conn.ExecuteAsync("delete from images where id = @OldImageId",
                    new
                    {
                        OldImageId = oldImageId
                    });

                var s3Filename = await conn.QuerySingleOrDefaultAsync<string>("select s3_filename from images where id = @OldImageId", new { oldImageId });

                await _s3ImageHandler.RemoveImageFromS3(s3Filename);
            }

            return request.ImageId;
        }
    }
}