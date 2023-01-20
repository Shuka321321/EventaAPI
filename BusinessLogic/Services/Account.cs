using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Identity;
using Models.Account;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Models.Mail;
using Microsoft.AspNetCore.WebUtilities;
using DataAccess.EF;

namespace BusinessLogic.Services
{
    public class Account : IAccount
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _config;

        public Account(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration) 
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = configuration;
        }
        
        public async Task<IdentityResult> Register(RegisterModel model) 
        {
            try
            {
                ApplicationUser user = new ApplicationUser()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    UserName = model.Email
                };

                return await _userManager.CreateAsync(user, model.Password);
            }
            catch (Exception ex)
            {
                return new IdentityResult();
            }
        }

        public async Task<string> Login(LoginModel model) 
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
                
                if (!result.Succeeded)
                {
                    return string.Empty;
                }

                var user = await _userManager.FindByNameAsync(model.Email);
                if (!user.EmailConfirmed) 
                {
                    return string.Empty;
                }

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, model.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var authSigninKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _config["JWT:ValidIssuer"],
                    audience: _config["JWT:ValidAudience"],
                    expires: DateTime.Now.AddDays(1),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256Signature)
                    );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public async Task<string> GenerateEmailConfirmationToken(RegisterModel model) 
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.Email);

                return await _userManager.GenerateEmailConfirmationTokenAsync(user);
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public async Task<bool> ConfirmEmail(string email, string token) 
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token)) 
                {
                    return false;
                }

                var user = await _userManager.FindByNameAsync(email);
                
                if (user == null) 
                {
                    return false;
                }
                var result = await _userManager.ConfirmEmailAsync(user, token);

                if (result.Succeeded) 
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<string> GetUserIdByEmail(string email) 
        {
            ApplicationUser user = await _userManager.FindByNameAsync(email);
            return user.Id;
        }

        public async Task<ApplicationUser> GetUserById(string userId) 
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<ApplicationUser> GetUserByEmail(string email) 
        {
            return await _userManager.FindByNameAsync(email);
        }

        public async Task<string> GeneratePasswordResetToken(ApplicationUser user) 
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<bool> UpdatePassword(ResetPasswordModel model) 
        {
            try
            {
                var user = await GetUserByEmail(model.Email);
                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded) 
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
