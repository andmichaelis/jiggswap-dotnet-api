using Jiggswap.Application.Users.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jiggswap.Api.ResponseModels
{
    public class AuthorizedUserResponseWithToken : AuthorizedUserResponse
    {
        public string Token { get; set; }
    }
}
