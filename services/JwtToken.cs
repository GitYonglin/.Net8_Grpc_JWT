using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace services
{
  public static class JwtToken
  {
    public static JwtSecurityTokenHandler JwtTokenHandler = new JwtSecurityTokenHandler();
    public static SymmetricSecurityKey SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("jdsiohoierbneuhosihdoifafoefe3439y263784fjdslifndsife839237hfi7yvrt32841764"));

    public static string GenerateJwtToken(string name)
    {
      if (string.IsNullOrEmpty(name))
      {
        throw new InvalidOperationException("Name is not specified.");
      }
      var role = new Claim(ClaimTypes.Role, "");
      if (name == "admin")
      {
        role = new Claim(ClaimTypes.Role, "Admin");
      }
      else if (name == "user")
      {
        role = new Claim(ClaimTypes.Role, "User");
      }

      var claims = new[] {
        new Claim(ClaimTypes.Name, name),
        role,
      };
      var credentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
      var token = new JwtSecurityToken("ExampleServer", "ExampleClients", claims, expires: DateTime.Now.AddDays(6), signingCredentials: credentials);
      return JwtTokenHandler.WriteToken(token);
    }
  }
}
