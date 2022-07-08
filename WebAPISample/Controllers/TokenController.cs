using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebAPISample.DTO;
using WebAPISample.Infrastructure.JWT;

namespace WebAPISample.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IJwtTokenService _tokenService;

        public TokenController(IJwtTokenService tokenService)
        {
            _tokenService = tokenService;
        }

        // attributed based routing feature
        [HttpPost("token")]
        public IActionResult CreateToken(LoginDto loginDto)
        {


            if(loginDto.Email == "test@test.com" && loginDto.Password == "123456")
            {
        var identity = new ClaimsIdentity(new Claim[]
    {
                    new Claim(ClaimTypes.HomePhone,"0212 500 45 78"),
                    new Claim(ClaimTypes.Name, loginDto.Email),
                    new Claim(ClaimTypes.Role, "Admin")
    });


                var token = _tokenService.CreateAccessToken(identity);

                return Ok(token);

            }

            return Unauthorized();

        }
    }
}
