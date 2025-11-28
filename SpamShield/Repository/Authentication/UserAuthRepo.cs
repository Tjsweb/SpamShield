using InstaHyreSDETest.Constants;
using InstaHyreSDETest.Data;
using InstaHyreSDETest.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InstaHyreSDETest.Repository.Authentication
{
    public interface IUserAuthRepo
    {
        Task<JWTResponseDTO> createJWTToken(string userName);
        Task<string> registerUser(string userName);
    }

    public class UserAuthRepo : IUserAuthRepo
    {
        private readonly UserManager<AppUser> userManager;

        public UserAuthRepo(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<string> registerUser(string userName)
        {
            try
            {
                AppUser user = new AppUser
                {
                    UserName = userName,
                    Email = userName,
                    EmailConfirmed = true,
                };

                IdentityResult result = await userManager.CreateAsync(user);

                if (result.Succeeded)
                {
                    return user.UserName;
                }
                else
                {
                    throw new Exception(result?.Errors?.FirstOrDefault()?.Description);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<JWTResponseDTO> createJWTToken(string userName)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(InstaHyreTokens.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, userName)
            };

            var token = new JwtSecurityToken(
                InstaHyreTokens.Issuer,
                InstaHyreTokens.Audience,
                claims,
                /* expires: DateTime.UtcNow.AddMinutes(30),*/
                expires: DateTime.UtcNow.AddMinutes(720),
                signingCredentials: creds
                );

            JWTResponseDTO jWTResponseModel = new JWTResponseDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                phoneNumber = userName,
                expiration = token.ValidTo,
            };

            return jWTResponseModel;
        }
    }
}
