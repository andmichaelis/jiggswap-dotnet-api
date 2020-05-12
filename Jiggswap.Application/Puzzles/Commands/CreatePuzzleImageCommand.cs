using Dapper;
using Jiggswap.Application.Common;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jiggswap.Application.Puzzles.Commands
{
    public class CreatePuzzleImageCommand : IRequest<bool>
    {
        public IFormFile ImageBlob { get; set; }

        public Guid PuzzleId { get; set; }
    }

    public class CreatePuzzleImageCommandHandler : IRequestHandler<CreatePuzzleImageCommand, bool>
    {
        private readonly IJiggswapDb _db;

        public CreatePuzzleImageCommandHandler(IJiggswapDb db)
        {
            _db = db;
        }

        private async Task<byte[]> GetImageDataFromBlob(IFormFile blob)
        {
            using var stream = new MemoryStream();

            await blob.CopyToAsync(stream).ConfigureAwait(false);

            return stream.ToArray();
        }

        public async Task<bool> Handle(CreatePuzzleImageCommand request, CancellationToken cancellationToken)
        {
            if (request.ImageBlob == null)
            {
                return true;
            }

            var imageData = await GetImageDataFromBlob(request.ImageBlob).ConfigureAwait(false);

            using var conn = _db.GetConnection();

            var oldImageId = await conn.QuerySingleOrDefaultAsync<int?>(
                "select image_id from puzzles where public_id = @PuzzleId", new
                {
                    request.PuzzleId
                }).ConfigureAwait(false);

            var newImageId = await conn.QuerySingleAsync<int>("insert into images (image_data) values (@ImageData) returning id",
                new
                {
                    imageData
                }).ConfigureAwait(false);

            await conn.ExecuteAsync("update puzzles set image_id = @ImageId where public_id = @PuzzleId",
                new
                {
                    ImageId = newImageId,
                    request.PuzzleId
                }).ConfigureAwait(false);

            if (oldImageId != null)
            {
                await conn.ExecuteAsync("delete from images where id = @OldImageId",
                    new
                    {
                        OldImageId = oldImageId
                    }).ConfigureAwait(false);
            }

            return true;
        }
    }
}
