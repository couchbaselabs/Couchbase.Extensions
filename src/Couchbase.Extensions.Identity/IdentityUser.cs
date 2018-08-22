using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Couchbase.Linq.Filters;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;


namespace Couchbase.Extensions.Identity
{
    [DocumentTypeFilter("identityuser")]
    public class IdentityUser
    {
        public IdentityUser()
		{
			Roles = new List<string>();
			Logins = new List<IdentityUserLogin>();
			Claims = new List<IdentityUserClaim>();
			Tokens = new List<IdentityUserToken>();
		}

		public string Id { get; set; }

        [JsonProperty("type")] public string Type => "identityuser";

        public string UserName { get; set; }

		public string NormalizedUserName { get; set; }

		/// <summary>
		/// A random value that must change whenever a users credentials change (password changed, login removed)
		/// </summary>
		public string SecurityStamp { get; set; }

		public string Email { get; set; }

		public string NormalizedEmail { get; set; }

		public bool EmailConfirmed { get; set; }

		public string PhoneNumber { get; set; }

		public bool PhoneNumberConfirmed { get; set; }

		public bool TwoFactorEnabled { get; set; }

		public DateTime? LockoutEndDateUtc { get; set; }

		public bool LockoutEnabled { get; set; }

		public int AccessFailedCount { get; set; }

        public string PasswordHash { get; set; }

        public List<string> Roles { get; set; }

        public List<IdentityUserToken> Tokens { get; set; }

        public List<IdentityUserClaim> Claims { get; set; }

        public List<IdentityUserLogin> Logins { get; set; }

		public virtual void AddRole(string role)
		{
			Roles.Add(role);
		}

		public virtual void RemoveRole(string role)
		{
			Roles.Remove(role);
		}

		public virtual void AddLogin(UserLoginInfo login)
		{
			Logins.Add(new IdentityUserLogin(login));
		}

		public virtual void RemoveLogin(string loginProvider, string providerKey)
		{
			Logins.RemoveAll(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey);
		}

		public virtual bool HasPassword()
		{
			return false;
		}

		public virtual void AddClaim(Claim claim)
		{
			Claims.Add(new IdentityUserClaim(claim));
		}

		public virtual void RemoveClaim(Claim claim)
		{
			Claims.RemoveAll(c => c.Type == claim.Type && c.Value == claim.Value);
		}

		public virtual void ReplaceClaim(Claim existingClaim, Claim newClaim)
		{
			var claimExists = Claims
				.Any(c => c.Type == existingClaim.Type && c.Value == existingClaim.Value);
			if (!claimExists)
			{
				// note: nothing to update, ignore, no need to throw
				return;
			}
			RemoveClaim(existingClaim);
			AddClaim(newClaim);
		}

		private IdentityUserToken GetToken(string loginProvider, string name)
			=> Tokens
				.FirstOrDefault(t => t.LoginProvider == loginProvider && t.Name == name);

		public virtual void SetToken(string loginProvider, string name, string value)
		{
			var existingToken = GetToken(loginProvider, name);
			if (existingToken != null)
			{
				existingToken.Value = value;
				return;
			}

			Tokens.Add(new IdentityUserToken
			{
				LoginProvider = loginProvider,
				Name = name,
				Value = value
			});
		}

		public virtual string GetTokenValue(string loginProvider, string name)
		{
			return GetToken(loginProvider, name)?.Value;
		}

		public virtual void RemoveToken(string loginProvider, string name)
		{
			Tokens.RemoveAll(t => t.LoginProvider == loginProvider && t.Name == name);
		}

		public override string ToString() => UserName;
    }
}
