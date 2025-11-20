using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace eology.ToolsCollection.TwoFactor
{ 
     // Definiert den Namen des Remote-Dienstes für die Client-Proxy-Generierung
    [Area("twoFactor")] // Optional: Definiert einen Bereich für den Controller

    public class TwoFactorAuthController : AbpController, ITwoFactorAuthAppService
    {
        private readonly ITwoFactorAuthAppService _twoFactorAuthAppService;

        public TwoFactorAuthController(ITwoFactorAuthAppService twoFactorAuthAppService)
        {
            _twoFactorAuthAppService = twoFactorAuthAppService;
        }

        [HttpGet]

        public Task<TotpSetupDto> BeginTotpSetupAsync()
        {
            return _twoFactorAuthAppService.BeginTotpSetupAsync();
        }

        [HttpPost]

        public Task<bool> FinalizeTotpSetupAsync(VerifyTotpSetupDto input)
        {
            return _twoFactorAuthAppService.FinalizeTotpSetupAsync(input);
        }

        [HttpPost]

        public Task<bool> VerifyTotpLoginAsync(VerifyTotpCodeDto input)
        {
            return _twoFactorAuthAppService.VerifyTotpLoginAsync(input);
        }

        [HttpPost]

        public Task<GenerateRecoveryCodesResultDto> GenerateNewRecoveryCodesAsync()
        {
            return _twoFactorAuthAppService.GenerateNewRecoveryCodesAsync();
        }

        [HttpGet]

        public Task<List<RecoveryCodeDto>> GetRecoveryCodesAsync()
        {
            return _twoFactorAuthAppService.GetRecoveryCodesAsync();
        }

        [HttpPost]

        public Task DisableTotpAsync()
        {
            return _twoFactorAuthAppService.DisableTotpAsync();
        }

        [HttpPost]

        public Task<bool> VerifyRecoveryCodeLoginAsync(string recoveryCode)
        {
            return _twoFactorAuthAppService.VerifyRecoveryCodeLoginAsync(recoveryCode);
        }
    }

}


