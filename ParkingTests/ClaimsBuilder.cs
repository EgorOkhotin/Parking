using System.Security.Claims;

namespace ParkingTests
{
    public class ClaimsBuilder
    {
        public ClaimsPrincipal CreateUser(string name, string role)
        {
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[]{
                    new Claim(ClaimTypes.Name, name),
                    new Claim(ClaimTypes.Role, role)
                }));
            return user;
        }
    }
}