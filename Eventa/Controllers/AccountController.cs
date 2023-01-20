using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using Models.Account;
using Models.Mail;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;

using System.Web;
using Microsoft.AspNetCore.Authorization;

namespace Eventa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {

        private readonly IAccount _accountService;
        private readonly IMail _mailService;
        private readonly IPerson _personService;

        public AccountController(IAccount accountService, IMail mailService, IPerson personService)
        {
            _accountService = accountService;
            _mailService = mailService;
            _personService = personService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return Unauthorized();
            }

            string token = await _accountService.Login(model);

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized();
            }

            string userId = await _accountService.GetUserIdByEmail(model.Email);

            if (string.IsNullOrEmpty(userId) || !_personService.PersonIsActive(userId)) 
            {
                return Unauthorized();
            }

            return Ok(token);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return Unauthorized();
            }
            var result = await _accountService.Register(model);

            if (result.Succeeded)
            {
                ApplicationUser user = await _accountService.GetUserByEmail(model.Email);

                bool person = await _personService.AddPerson(user);

                string token = await _accountService.GenerateEmailConfirmationToken(model);
                token = HttpUtility.UrlEncode(token);
                
                string callback_url = Request.Scheme + "://" + Request.Host + Url.Action("ConfirmEmail", "Account", new { email = model.Email, token = token });

                MailRequest mail = new MailRequest()
                {
                    ToEmail = model.Email,
                    Body = "Please confirm your email address: <a href='" + callback_url + "'>Click Here</a>",
                    Subject = "Email Confirmation"
                };

                bool send = await _mailService.SendEmailAsync(mail);
                if (!send)
                {
                    return BadRequest();
                }
                return Ok(result);
            }
            return Unauthorized();
        }

        [Route("ConfirmEmail")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            token = HttpUtility.UrlDecode(token);
            if (!await _accountService.ConfirmEmail(email, token))
            {
                return BadRequest();
            }

            string userId = await _accountService.GetUserIdByEmail(email);
            if (string.IsNullOrEmpty(userId)) 
            {
                return BadRequest();
            }

            bool activate = await _personService.ActivatePerson(userId);

            if (!activate) 
            {
                return BadRequest();
            }
            return Ok();
        }

        [HttpPost("forgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(string email) 
        {
            if (string.IsNullOrEmpty(email)) 
            {
                return BadRequest();
            }

            ApplicationUser user = await _accountService.GetUserByEmail(email);

            if (user == null) 
            {
                return BadRequest();
            }

            string token = await _accountService.GeneratePasswordResetToken(user);

            token = HttpUtility.UrlEncode(token);
            string callback_url = Request.Scheme + "://" + Request.Host + Url.Action("ResetPassword", "Account", new { email = email, token = token });

            MailRequest mail = new MailRequest()
            {
                ToEmail = email,
                Body = "To reset the password <a href='" + callback_url + "'>Click Here</a>",
                Subject = "Reset Password"
            };

            bool send = await _mailService.SendEmailAsync(mail);
            if (!send)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model) 
        {
            if (!ModelState.IsValid) 
            {
                return BadRequest();
            }
            model.Token = HttpUtility.UrlDecode(model.Token);

            bool update = await _accountService.UpdatePassword(model);

            if (!update) 
            {
                return BadRequest();
            }

            return Ok();
        }

    }
}