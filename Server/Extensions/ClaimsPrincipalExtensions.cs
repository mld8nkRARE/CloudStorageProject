using System.Security.Claims;

namespace Server.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)
                       ?? user.FindFirst("sub")
                       ?? user.FindFirst("id");

            if (idClaim == null)
                throw new UnauthorizedAccessException("User ID not found in token");

            return Guid.Parse(idClaim.Value);
        }
        /*
        public static Guid? GetUserIdOrNull(this ClaimsPrincipal user)
        {
            var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)
                       ?? user.FindFirst("sub")
                       ?? user.FindFirst("id");

            if (idClaim == null || !Guid.TryParse(idClaim.Value, out var id))
                return null;

            return id;
        }
        */
    }
}
