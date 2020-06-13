using Dapper;
using ImageMagick;
using Jiggswap.Application.Common;
using Jiggswap.Application.Images;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Jiggswap.Application.Puzzles.Commands
{
    public class CreatePuzzleImageCommand : IRequest<bool>
    {
        public int ImageId { get; set; }

        public Guid PuzzleId { get; set; }
    }

    public class CreatePuzzleImageCommandHandler : IRequestHandler<CreatePuzzleImageCommand, bool>
    {
        private readonly IJiggswapDb _db;
        private readonly IS3ImageHandler _s3ImageHandler;

        public CreatePuzzleImageCommandHandler(IJiggswapDb db, IS3ImageHandler s3ImageHandler)
        {
            _db = db;
            _s3ImageHandler = s3ImageHandler;
        }

        public async Task<bool> Handle(CreatePuzzleImageCommand request, CancellationToken cancellationToken)
        {
            using var conn = _db.GetConnection();

            var oldImageId = await conn.QuerySingleOrDefaultAsync<int?>(
                "select image_id from puzzles where public_id = @PuzzleId", new
                {
                    request.PuzzleId
                });

            await conn.ExecuteAsync("update puzzles set image_id = @ImageId where public_id = @PuzzleId",
            new
            {
                request.ImageId,
                request.PuzzleId
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

            return true;
        }
    }
}