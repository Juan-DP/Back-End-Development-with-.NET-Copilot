using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagementAPI.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token) || !ValidateToken(token))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized: Invalid or missing token.");
                return;
            }

            await _next(context);
        }

        private bool ValidateToken(string token)
        {
            try
            {
                var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;

                // Add your token validation logic here (e.g., checking issuer, audience, expiration, etc.)
                if (jwtToken == null || jwtToken.ValidTo < System.DateTime.UtcNow)
                {
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}