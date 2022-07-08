using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace WebAPISample.Infrastructure.JWT
{
    public interface IJwtTokenService
    {
        string CreateAccessToken(ClaimsIdentity identity);
    }
}
