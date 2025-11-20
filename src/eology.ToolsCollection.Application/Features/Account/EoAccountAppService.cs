using System.Threading.Tasks;
using eology.ToolsCollection.Localization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.Account.Emailing;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;

namespace eology.ToolsCollection.Features.Account
{
    public class EoAccountAppService : AccountAppService, ITransientDependency
    {
        private readonly IConfiguration _configuration;
		private readonly IStringLocalizer<ToolsCollectionResource> _localizer;

		public EoAccountAppService(
			IIdentityUserRepository userRepository,
			IdentityUserManager userManager,
			IIdentityRoleRepository roleRepository,
			IAccountEmailer accountEmailer,
			IdentitySecurityLogManager securityLogManager,
			Microsoft.Extensions.Options.IOptions<IdentityOptions> identityOptions,
			IConfiguration configuration,
			IStringLocalizer<ToolsCollectionResource> localizer
			)
			: base(userManager, roleRepository, accountEmailer, securityLogManager, identityOptions)
		{
			_configuration = configuration;
			_localizer = localizer;
        }

        public override Task<IdentityUserDto> RegisterAsync(RegisterDto input)
        {
            // Steuert die Registrierung über appsettings.json
            bool isSelfRegistrationEnabled = _configuration.GetValue<bool>("Abp.Account.IsSelfRegistrationEnabled");
            if (!isSelfRegistrationEnabled)
            {
                throw new UserFriendlyException(_localizer["Global:RegistrationDisabledMessage"]);
            }

            // Optional: Call base method, falls Registrierung aktiviert ist
            return base.RegisterAsync(input);
        }
    }
}
