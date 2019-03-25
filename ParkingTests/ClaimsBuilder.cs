using System.Security.Claims;
using System.Security.Principal;

namespace ParkingTests
{
    public class ClaimsBuilder
    {
        public ClaimsPrincipal CreateUser(string name, string role)
        {
            // var user = new ClaimsPrincipal(
            //     new ClaimsIdentity(new Claim[]{
            //         new Claim(ClaimTypes.Name, name),
            //         new Claim(ClaimTypes.Role, role)
            //     }));
            var identity = new GenericIdentity($"{name}");
            //identity.AddClaim(new Claim(ClaimTypes.Role, role));
            var principal = new GenericPrincipal(identity,new string[]{role});
            return principal;
        }
    }
}