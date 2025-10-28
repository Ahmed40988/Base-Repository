using System.Security.Claims;

namespace Web.APIs.Abstractions
{
    public static class UserExtensions
    {
        public static string GetUserId(this ClaimsPrincipal user)
       => user.FindFirstValue(ClaimTypes.NameIdentifier);
    }

}
