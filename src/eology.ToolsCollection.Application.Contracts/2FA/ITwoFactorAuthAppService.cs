using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace eology.ToolsCollection.TwoFactor
{
    public interface ITwoFactorAuthAppService : IApplicationService
    {
        Task<TotpSetupDto> BeginTotpSetupAsync();
        Task<bool> FinalizeTotpSetupAsync(VerifyTotpSetupDto input);
        Task<bool> VerifyTotpLoginAsync(VerifyTotpCodeDto input);
        Task<GenerateRecoveryCodesResultDto> GenerateNewRecoveryCodesAsync();
        Task<List<RecoveryCodeDto>> GetRecoveryCodesAsync();
        Task DisableTotpAsync();
        Task<bool> VerifyRecoveryCodeLoginAsync(string recoveryCode);
    }
}

