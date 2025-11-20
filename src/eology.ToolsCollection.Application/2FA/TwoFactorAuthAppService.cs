using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Volo.Abp.Identity;

using Microsoft.Extensions.Options;
using OtpNet;
using Volo.Abp.Application.Services;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Security.Encryption; // Für IStringEncryptionService
using Volo.Abp.Domain.Repositories;
using eology.ToolsCollection.Domain.Shared;
using Volo.Abp.Users;
using Volo.Abp.Guids; // Für IGuidGenerator
using Volo.Abp.Features; // Für Feature-Management, falls benötigt
using System.Text.Json;
using Volo.Abp;
using Volo.Abp.Data;

namespace eology.ToolsCollection.TwoFactor
{
    public class TwoFactorAuthAppService : ApplicationService, ITwoFactorAuthAppService
    {
        private readonly IdentityUserManager _userManager;
        private readonly IIdentityUserRepository _identityUserRepository;
        private readonly IStringEncryptionService _stringEncryptionService;
        private readonly IGuidGenerator _guidGenerator;
        private readonly SignInManager<Volo.Abp.Identity.IdentityUser> _signInManager; // Für 2FA-bezogene Methoden

        public TwoFactorAuthAppService(
            IdentityUserManager userManager,
            IIdentityUserRepository identityUserRepository,
            IStringEncryptionService stringEncryptionService,
            IGuidGenerator guidGenerator,
            SignInManager<Volo.Abp.Identity.IdentityUser> signInManager)
        {
            _userManager = userManager;
            _identityUserRepository = identityUserRepository;
            _stringEncryptionService = stringEncryptionService;
            _guidGenerator = guidGenerator;
            _signInManager = signInManager;
        }

        public async Task<TotpSetupDto> BeginTotpSetupAsync()
        {

            var currentUser = await _userManager.GetByIdAsync((Guid)CurrentUser.Id);
            //var currentUser = await _userManager.GetUserAsync(CurrentUser.To<Volo.Abp.Identity.IdentityUser>());

            if (currentUser == null)
            {
                throw new UserFriendlyException("Benutzer nicht gefunden!");
            }

            // Generieren eines neuen geheimen Schlüssels
            byte[] secretBytes = KeyGeneration.GenerateRandomKey(20); // 160 Bit
            string secretKeyBase32 = Base32Encoding.ToString(secretBytes);

            // Verschlüsseln und Speichern des temporären Geheimnisses
            // Dies ist ein temporäres Geheimnis, es wird nach der Verifizierung dauerhaft gespeichert
            currentUser.SetProperty(UserConsts.TotpSecretPropertyName, _stringEncryptionService.Encrypt(secretKeyBase32));
            await _identityUserRepository.UpdateAsync(currentUser);

            // QR-Code-URI generieren
            // Format: otpauth://totp/{Issuer}:{AccountName}?secret={SecretKey}&issuer={Issuer}&digits={Digits}&period={Period}
            // Issuer ist Ihr Anwendungsname, AccountName ist typischerweise die E-Mail/der Benutzername des Benutzers
            var issuer = "YourAppName"; // Ersetzen Sie dies durch Ihren Anwendungsnamen
            var accountName = currentUser.UserName; // Oder currentUser.Email
            var qrCodeUri = $"otpauth://totp/{issuer}:{accountName}?secret={secretKeyBase32}&issuer={issuer}&digits=6&period=30";

            return new TotpSetupDto
            {
                SecretKey = secretKeyBase32, // Den rohen Schlüssel für die manuelle Eingabe bereitstellen
                QrCodeUri = qrCodeUri,
                ManualEntryKey = secretKeyBase32 // Dasselbe wie SecretKey für die manuelle Eingabe
            };
        }

        public async Task<bool> FinalizeTotpSetupAsync(VerifyTotpSetupDto input)
        {
                var currentUser = await _userManager.GetByIdAsync((Guid)CurrentUser.Id);
            //var currentUser = await _userManager.GetUserAsync(CurrentUser.To<IdentityUser>());
            if (currentUser == null)
            {
                throw new UserFriendlyException("Benutzer nicht gefunden!");
            }

            var encryptedSecret = currentUser.GetProperty<string>(UserConsts.TotpSecretPropertyName);
            if (string.IsNullOrEmpty(encryptedSecret))
            {
                throw new UserFriendlyException("TOTP-Einrichtung nicht initialisiert.");
            }

            var secretKeyBase32 = _stringEncryptionService.Decrypt(encryptedSecret);
            var totp = new Totp(Base32Encoding.ToBytes(secretKeyBase32));

            // Code mit einem kleinen Fenster für Zeitversatz verifizieren
            bool isValid = totp.VerifyTotp(
                input.Code,
                out long timeStepMatched,
                new VerificationWindow(previous: 1, future: 1) // 1 Schritt Abweichung zulassen
            );

            if (isValid)
            {
                // 2FA für den Benutzer in ASP.NET Core Identity aktivieren
                var result = await _userManager.SetTwoFactorEnabledAsync(currentUser, true);
                if (!result.Succeeded)
                {
                    throw new UserFriendlyException("Fehler beim Aktivieren der Zwei-Faktor-Authentifizierung.");
                }

                // Das Geheimnis dauerhaft speichern (es ist bereits in BeginTotpSetupAsync verschlüsselt)
                // Es ist nicht notwendig, die Geheimnis-Eigenschaft erneut zu speichern, sie ist bereits im Benutzerobjekt.
                await _identityUserRepository.UpdateAsync(currentUser);

                // Anfängliche Wiederherstellungscodes generieren
                await GenerateNewRecoveryCodesInternalAsync(currentUser);

                return true;
            }
            else
            {
                // Temporäres Geheimnis löschen, wenn die Verifizierung fehlschlägt
                currentUser.SetProperty(UserConsts.TotpSecretPropertyName, (string)null);
                await _identityUserRepository.UpdateAsync(currentUser);
                return false;
            }
        }

        public async Task<bool> VerifyTotpLoginAsync(VerifyTotpCodeDto input)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new UserFriendlyException("Zwei-Faktor-Authentifizierungsbenutzer konnte nicht geladen werden.");
            }

            var encryptedSecret = user.GetProperty<string>(UserConsts.TotpSecretPropertyName);
            if (string.IsNullOrEmpty(encryptedSecret))
            {
                // Dieses Szenario sollte nicht auftreten, wenn RequiresTwoFactor true ist, aber als Schutzmaßnahme
                throw new UserFriendlyException("TOTP ist für diesen Benutzer nicht konfiguriert.");
            }

            var secretKeyBase32 = _stringEncryptionService.Decrypt(encryptedSecret);
            var totp = new Totp(Base32Encoding.ToBytes(secretKeyBase32));

            // Code verifizieren
            bool isValid = totp.VerifyTotp(
                input.Code,
                out long timeStepMatched,
                new VerificationWindow(previous: 1, future: 1)
            );

            if (isValid)
            {
                // Benutzer nach erfolgreicher 2FA anmelden
                await _signInManager.TwoFactorAuthenticatorSignInAsync(input.Code, false, false); // rememberClient: false, isPersistent: false
                return true;
            }
            else
            {
                // Fehlgeschlagene Zugriffsversuche erhöhen, falls benötigt, Sperrung verwalten
                await _userManager.AccessFailedAsync(user);
                return false;
            }
        }

        public async Task DisableTotpAsync()
        {
                var currentUser = await _userManager.GetByIdAsync((Guid)CurrentUser.Id);
            //var currentUser = await _userManager.GetUserAsync(CurrentUser.To<IdentityUser>());
            if (currentUser == null)
            {
                throw new UserFriendlyException("Benutzer nicht gefunden!");
            }

            // 2FA in ASP.NET Core Identity deaktivieren
            var result = await _userManager.SetTwoFactorEnabledAsync(currentUser, false);
            if (!result.Succeeded)
            {
                throw new UserFriendlyException("Fehler beim Deaktivieren der Zwei-Faktor-Authentifizierung.");
            }

            // Geheimnis und Wiederherstellungscodes löschen
            currentUser.SetProperty(UserConsts.TotpSecretPropertyName, (string)null);
            currentUser.SetProperty(UserConsts.RecoveryCodesPropertyName, (string)null);
            await _identityUserRepository.UpdateAsync(currentUser);
        }

        public async Task<GenerateRecoveryCodesResultDto> GenerateNewRecoveryCodesAsync()
        {
                var currentUser = await _userManager.GetByIdAsync((Guid)CurrentUser.Id);
            //var currentUser = await _userManager.GetUserAsync(CurrentUser.To<IdentityUser>());
            if (currentUser == null)
            {
                throw new UserFriendlyException("Benutzer nicht gefunden!");
            }

            if (!await _userManager.GetTwoFactorEnabledAsync(currentUser))
            {
                throw new UserFriendlyException("Zwei-Faktor-Authentifizierung ist für diesen Benutzer nicht aktiviert.");
            }

            var recoveryCodes = await GenerateNewRecoveryCodesInternalAsync(currentUser);
            return new GenerateRecoveryCodesResultDto { RecoveryCodes = recoveryCodes };
        }

        public async Task<List<RecoveryCodeDto>> GetRecoveryCodesAsync()
        {
            var currentUser = await _userManager.GetByIdAsync((Guid)CurrentUser.Id);
            //var currentUser = await _userManager.GetUserAsync(CurrentUser.To<IdentityUser>());
            if (currentUser == null)
            {
                throw new UserFriendlyException("Benutzer nicht gefunden!");
            }

            var encryptedCodesJson = currentUser.GetProperty<string>(UserConsts.RecoveryCodesPropertyName);
            if (string.IsNullOrEmpty(encryptedCodesJson))
            {
                return new List<RecoveryCodeDto>();
            }

            var decryptedCodesJson = _stringEncryptionService.Decrypt(encryptedCodesJson);
            var codes = JsonSerializer.Deserialize<List<RecoveryCodeDto>>(decryptedCodesJson);
            return codes ?? new List<RecoveryCodeDto>();
        }

        public async Task<bool> VerifyRecoveryCodeLoginAsync(string recoveryCode)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new UserFriendlyException("Zwei-Faktor-Authentifizierungsbenutzer konnte nicht geladen werden.");
            }

            // ASP.NET Core Identity's SignInManager übernimmt die Verifizierung des Wiederherstellungscodes
            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            if (result.Succeeded)
            {
                // Wiederherstellungscodes aktualisieren, um den verwendeten zu markieren
                var encryptedCodesJson = user.GetProperty<string>(UserConsts.RecoveryCodesPropertyName);
                if (!string.IsNullOrEmpty(encryptedCodesJson))
                {
                    var decryptedCodesJson = _stringEncryptionService.Decrypt(encryptedCodesJson);
                    var codes = JsonSerializer.Deserialize<List<RecoveryCodeDto>>(decryptedCodesJson);
                    var usedCode = codes.FirstOrDefault(c => c.Code == recoveryCode && !c.Used);
                    if (usedCode != null)
                    {
                        usedCode.Used = true;
                        user.SetProperty(UserConsts.RecoveryCodesPropertyName, _stringEncryptionService.Encrypt(JsonSerializer.Serialize(codes)));
                        await _identityUserRepository.UpdateAsync(user);
                    }
                }
                return true;
            }
            else if (result.IsLockedOut)
            {
                throw new UserFriendlyException("Benutzerkonto gesperrt.");
            }
            else if (result.IsNotAllowed)
            {
                throw new UserFriendlyException("Anmeldung nicht erlaubt.");
            }
            else
            {
                throw new UserFriendlyException("Ungültiger Wiederherstellungscode.");
            }
        }

        private async Task<List<string>> GenerateNewRecoveryCodesInternalAsync(Volo.Abp.Identity.IdentityUser user)
        {
            // ASP.NET Core Identity bietet eine Möglichkeit zur Generierung von Wiederherstellungscodes
            // Diese werden jedoch intern von Identity verwaltet.
            // Für eine benutzerdefinierte Lösung generieren und verwalten wir unsere eigenen.
            // Alternativ, wenn Sie die integrierten Wiederherstellungscodes von Identity nutzen möchten,
            // müssten Sie diese über einen benutzerdefinierten API-Endpunkt verfügbar machen.
            // Für diese benutzerdefinierte Lösung generieren und verwalten wir sie manuell.

            var recoveryCodes = new List<string>();
            var random = new Random();
            for (int i = 0; i < 10; i++) // 10 Wiederherstellungscodes generieren
            {
                var code = _guidGenerator.Create().ToString("N").Substring(0, 10).ToUpperInvariant(); // Einfacher Zufallsstring
                recoveryCodes.Add(code);
            }

            // Als JSON-String nach der Verschlüsselung speichern
            var recoveryCodeDtos = recoveryCodes.Select(c => new RecoveryCodeDto { Code = c, Used = false }).ToList();
            user.SetProperty(UserConsts.RecoveryCodesPropertyName, _stringEncryptionService.Encrypt(JsonSerializer.Serialize(recoveryCodeDtos)));
            await _identityUserRepository.UpdateAsync(user);

            return recoveryCodes;
        }
    }
}