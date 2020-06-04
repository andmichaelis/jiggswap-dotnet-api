using Jiggswap.Application.OAuth.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jiggswap.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [AllowAnonymous]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public ActionResult GetHome()
        {
            return Ok();
        }
    }
}