using Dapper;
using Jiggswap.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Jiggswap.Application.Images.Queries
{
    public class GetImageQuery : IRequest<byte[]>
    {
        public int ImageId { get; set; }
    }

    public class GetImageQueryHandler : IRequestHandler<GetImageQuery, byte[]>
    {
        private readonly IJiggswapDb _db;

        public GetImageQueryHandler(IJiggswapDb db)
        {
            _db = db;
        }

        public async Task<byte[]> Handle(GetImageQuery request, CancellationToken cancellationToken)
        {
            using var conn = _db.GetConnection();

            return await conn.QuerySingleAsync<byte[]>("select image_data from images where id = @ImageId",
            new
            {
                request.ImageId
            }).ConfigureAwait(false);
        }
    }
}
