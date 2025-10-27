using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Security.Claims;

namespace Web.Infrastructure.Service
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<(string Token,int expiresIn)> GenerateTokenAsync(AppUser user, UserManager<AppUser> userManager)
        {
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var Roles = await userManager.GetRolesAsync(user);

            foreach (var Role in Roles)
            {
                userClaims.Add(new Claim(ClaimTypes.Role, Role));
            }

            var authKeyInByets = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));

            var JwtObject = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: userClaims,
                expires: DateTime.Now.AddMinutes(int.Parse(_configuration["JWT:ExpiryMinutes"])),
                signingCredentials: new SigningCredentials(authKeyInByets, SecurityAlgorithms.HmacSha256Signature)
            );
            var expiresIn = int.Parse(_configuration["JWT:ExpiryMinutes"]);

            return (token: new JwtSecurityTokenHandler().WriteToken(JwtObject), expiresIn: expiresIn * 60);
        }


        public string? ValidateToken(string token)
        {
            var TokenHandler = new JwtSecurityTokenHandler();
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));

            try
            {
                TokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    IssuerSigningKey = symmetricSecurityKey,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                }, out SecurityToken validatedToken
                );
                var jwtToken = (JwtSecurityToken)validatedToken;
                return jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;

            }
            catch (Exception ex)
            {
                return null;
            }

        }
    }
}
