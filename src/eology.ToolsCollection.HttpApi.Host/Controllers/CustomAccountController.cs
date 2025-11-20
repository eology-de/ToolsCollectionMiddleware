using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Identity;
using Volo.Abp.Account;
using eology.ToolsCollection.TwoFactor;
using Volo.Abp;
using IdentityUser = Volo.Abp.Identity.IdentityUser; // Ihre benutzerdefinierten DTOs

namespace eology.ToolsCollection.Controllers
{
    [Area("account")]

    public class CustomAccountController : AbpController
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IdentityUserManager _userManager;
        private readonly ITwoFactorAuthAppService _twoFactorAuthAppService; // Ihren benutzerdefinierten 2FA-Dienst injizieren

        public CustomAccountController(
            SignInManager<IdentityUser> signInManager,
            IdentityUserManager userManager,
            ITwoFactorAuthAppService twoFactorAuthAppService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _twoFactorAuthAppService = twoFactorAuthAppService;
        }

        [HttpPost]
        public async Task<IActionResult> LoginWith2Fa( LoginDto input) // LoginDto von AbpAccount
        {
            var result = await _signInManager.PasswordSignInAsync(
                input.UserNameOrEmailAddress,
                input.Password,
                input.RememberMe,
                lockoutOnFailure: true);

            if (result.RequiresTwoFactor)
            {
                // Benutzer benötigt 2FA. Einen spezifischen Status oder DTO zurückgeben, der dies anzeigt.
                // Das Frontend leitet dann zur 2FA-Verifizierungsseite weiter.
                return Ok(new { RequiresTwoFactor = true, Providers = await _userManager.GetValidTwoFactorProvidersAsync(await _signInManager.GetTwoFactorAuthenticationUserAsync()) });
            }
            else if (result.Succeeded)
            {
                // Benutzer erfolgreich ohne 2FA angemeldet
                return Ok(new { Succeeded = true });
            }
            else if (result.IsLockedOut)
            {
                return BadRequest(new { Error = "Benutzerkonto gesperrt." });
            }
            else if (result.IsNotAllowed)
            {
                return BadRequest(new { Error = "Anmeldung nicht erlaubt." });
            }
            else
            {
                return BadRequest(new { Error = "Ungültiger Anmeldeversuch." });
            }
        }

        [HttpPost]

        public async Task<IActionResult> Verify2FaCode( VerifyTotpCodeDto input)
        {
            try
            {
                var success = await _twoFactorAuthAppService.VerifyTotpLoginAsync(input);
                if (success)
                {
                    return Ok(new { Succeeded = true });
                }
                else
                {
                    return BadRequest(new { Error = "Ungültiger Verifizierungscode." });
                }
            }
            catch (UserFriendlyException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost]

        public async Task<IActionResult> VerifyRecoveryCode( string recoveryCode)
        {
            try
            {
                var success = await _twoFactorAuthAppService.VerifyRecoveryCodeLoginAsync(recoveryCode);
                if (success)
                {
                    return Ok(new { Succeeded = true });
                }
                else
                {
                    return BadRequest(new { Error = "Ungültiger Wiederherstellungscode." });
                }
            }
            catch (UserFriendlyException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}