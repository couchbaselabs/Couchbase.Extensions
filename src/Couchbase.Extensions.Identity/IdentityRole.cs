using System;
using System.Collections.Generic;
using System.Text;
using Couchbase.Linq.Filters;
using Newtonsoft.Json;

namespace Couchbase.Extensions.Identity
{
    [DocumentTypeFilter("identityrole")]
    public class IdentityRole
    {
        public IdentityRole()
        {
        }

        public IdentityRole(string roleName) : this()
        {
            Name = roleName;
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string NormalizedName { get; set; }

        [JsonProperty("type")] public string Type => "identityrole";

        public override string ToString() => Name;
    }
}
