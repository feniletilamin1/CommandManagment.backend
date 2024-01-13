using Azure.Core;
using CommandManagment.backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CommandManagment.backend.Helpers
{
    public class JwtService
    {
        readonly AppDbContext _context;
        public JwtService(AppDbContext context) { 
            _context = context;
        }

        public string GenerateToken(string email)
        {
            List<Claim> claims = new() { new Claim(ClaimTypes.Name, email) };
            JwtSecurityToken token = GenerateJwtSecureToken(claims, DateTime.UtcNow.Add(TimeSpan.FromDays(1)));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateInviteToken(int teamId)
        {
            List<Claim> claims = new() { new Claim("teamId", teamId.ToString()) };
            JwtSecurityToken token = GenerateJwtSecureToken(claims, DateTime.UtcNow.Add(TimeSpan.FromHours(5)));

            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenString;
        }

        private static JwtSecurityToken GenerateJwtSecureToken(List<Claim> claims, DateTime expires)
        {
            JwtSecurityToken JwtSecurityToken = new(
               issuer: AuthOptions.ISSUER,
               audience: AuthOptions.AUDIENCE,
               claims: claims,
               expires: expires,
               signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            return JwtSecurityToken;
        }

        public JwtSecurityToken? ValidateToken(string token)
        {
            JwtSecurityTokenHandler tokenHandler = new();
            JwtSecurityToken? jwtToken = null;

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                ValidateIssuer = true,
                ValidIssuer = AuthOptions.ISSUER,
                ValidateAudience = true,
                ValidAudience = AuthOptions.AUDIENCE,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                ClaimsPrincipal claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                jwtToken = (JwtSecurityToken)validatedToken;
                return jwtToken;
            }
            catch (SecurityTokenException)
            {
                _context.Invites.Where(p => p.Token == tokenHandler.WriteToken(jwtToken)).ExecuteDelete();
                return null;
            }
        }

        public string GetUserEmailFromJwt(string serverToken)
        {
            string[] headersAuth = serverToken.Split(" ");

            JwtSecurityToken validateToken = ValidateToken(headersAuth[1]);

            string userEmail = validateToken.Claims.First(claim => claim.Type == ClaimTypes.Name).Value;

            return userEmail;
        }

        public string GenerateRandomPassword(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var password = new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            return password;
        }
    }
}
