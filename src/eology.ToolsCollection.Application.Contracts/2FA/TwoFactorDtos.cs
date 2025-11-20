using Volo.Abp.Application.Dtos;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace eology.ToolsCollection.TwoFactor
{
    public class TotpSetupDto : EntityDto<Guid>
    {
        public string SecretKey { get; set; }
        public string QrCodeUri { get; set; }
        public string ManualEntryKey { get; set; }
    }

    public class VerifyTotpCodeDto
    {

        // TOTP-Codes sind typischerweise 6-stellig
        public string Code { get; set; }
    }

    public class VerifyTotpSetupDto : VerifyTotpCodeDto
    {

        public string SecretKey { get; set; }
    }

    public class RecoveryCodeDto
    {
        public string Code { get; set; }
        public bool Used { get; set; }
    }

    public class GenerateRecoveryCodesResultDto
    {
        public List<string> RecoveryCodes { get; set; }
    }
    public class LoginDto
    {
        // Aus Sicherheitsgründen, um das Passwort nicht zu protokollieren
        public string UserNameOrEmailAddress { get; set; }
        // Aus Sicherheitsgründen
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
