using Microsoft.AspNetCore.Identity;
using Models.Account;


namespace BusinessLogic.Interfaces
{
    public interface IAccount
    {
        Task<IdentityResult> Register(RegisterModel model);
        Task<string> Login(LoginModel model);
        Task<string> GenerateEmailConfirmationToken(RegisterModel user);
        Task<string> GeneratePasswordResetToken(ApplicationUser user);
        Task<bool> ConfirmEmail(string email, string token);
        Task<string> GetUserIdByEmail(string email);
        Task<ApplicationUser> GetUserById(string userId);
        Task<ApplicationUser> GetUserByEmail(string email);
        Task<bool> UpdatePassword(ResetPasswordModel model);
    }
}
