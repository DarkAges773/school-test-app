using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace WebApp
{
    [Route("api")]
    public class LoginController : Controller
    {
        private readonly IAccountDatabase _db;

        public LoginController(IAccountDatabase db)
        {
            _db = db;
        }

        [HttpPost("sign-in")]
        public async Task Login(string userName)
        {
            var account = await _db.FindByUserNameAsync(userName);
            if (account != null)
            {
				//TODO 1: Generate auth cookie for user 'userName' with external id
				var claims = new List<Claim>
			    {
				    new Claim(ClaimTypes.Name, account.UserName),
				    new Claim(ClaimTypes.Role, account.Role),
                    new Claim("ExternalId", account.ExternalId)
			    };
				ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
				await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
			}
            else
            {
				//TODO 2: return 404 if user not found
				// I did not want to change the signature of Login method.
				// I could have used "return NotFound()", 
				// but then well have to return some "IActionResult" for the successful login (for example "return Ok();")
				// and the method signature would become "Task<IActionResult> Login(string)".
				Response.StatusCode = 404;
                return;
            }
        }
    }
}