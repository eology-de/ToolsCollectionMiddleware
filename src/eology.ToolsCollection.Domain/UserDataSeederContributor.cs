using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;

namespace eology.ToolsCollection
{
	public class UserDataSeederContributor : IDataSeedContributor, ITransientDependency
	{
		private readonly IIdentityUserRepository _identityUserRepository;
		private readonly IdentityUserManager _identityUserManager;
		private readonly IdentityRoleManager _roleManager;

		public UserDataSeederContributor(
			IdentityUserManager identityUserManager,
			IIdentityUserRepository identityUserRepository,
			IdentityRoleManager roleManager
			)
		{
			_identityUserManager = identityUserManager;
			_identityUserRepository = identityUserRepository;
			_roleManager = roleManager;
		}

		public async Task SeedAsync(DataSeedContext context)
		{
			var roleNames = new[] { "admin" };
			for (int i = 0; i < roleNames.Length; i++)
			{
				var roleName = roleNames[i];
				if (await _roleManager.FindByNameAsync(roleName) == null)
				{
					var role = new IdentityRole(Guid.NewGuid(), roleName);
					role.IsStatic = true; // verhindert ungewolltes Löschen/Umbenennen
					if (roleName == "admin")
					{
						role.IsPublic = false;
						role.IsDefault = true; // macht die Rolle zur Standardrolle mit allen Rechten						
					}
					else
					{
						role.IsPublic = true; // macht die Rolle in der Rollen-Auswahl sichtbar
					}
					await _roleManager.CreateAsync(role);
				}
			}

			// Add users
			if (await _identityUserRepository.FindByNormalizedEmailAsync("s.mock@eology.de") == null)
			{
				IdentityUser smk = new(Guid.NewGuid(), "smk", "s.mock@eology.de");
				await _identityUserManager.CreateAsync(smk, "test123#SMK");
				await _identityUserManager.AddToRoleAsync(smk, roleNames[0]);
			}

			if (await _identityUserRepository.FindByNormalizedEmailAsync("s.schoen@eology.de") == null)
			{
				IdentityUser ssn = new(Guid.NewGuid(), "ssn", "s.schoen@eology.de");
				await _identityUserManager.CreateAsync(ssn, "test123#SSN");
				await _identityUserManager.AddToRoleAsync(ssn, roleNames[0]);
			}
		}
	}
}