using System.Security.Claims;
using Couchbase.Linq.Filters;

namespace Couchbase.Extensions.Identity
{
    /// <summary>
    /// A claim that a user possesses.
    /// </summary>
    [DocumentTypeFilter("identityuserclaim")]
    public class IdentityUserClaim
    {
        public IdentityUserClaim()
        {
        }

        public IdentityUserClaim(Claim claim)
        {
            Type = claim.Type;
            Value = claim.Value;
        }

        /// <summary>
        /// Claim type
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// Claim value
        /// </summary>
        public string Value { get; }

        public Claim ToSecurityClaim()
        {
            return new Claim(Type, Value);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is IdentityUserClaim that)) return false;
            return this.Type == that.Type && this.Value == that.Value;
        }

        protected bool Equals(IdentityUserClaim other)
        {
            return string.Equals(Type, other.Type) && string.Equals(Value, other.Value);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Type != null ? Type.GetHashCode() : 0) * 397) ^ (Value != null ? Value.GetHashCode() : 0);
            }
        }

        public static bool operator ==(IdentityUserClaim left, IdentityUserClaim right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(IdentityUserClaim left, IdentityUserClaim right)
        {
            return !Equals(left, right);
        }
    }
}
