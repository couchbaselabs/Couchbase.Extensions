using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Couchbase.Core;
using Couchbase.Extensions.DependencyInjection;
using Couchbase.Extensions.Identity;
using Couchbase.Linq;
using Couchbase.N1QL;
using Moq;
using Xunit;

namespace Couchbase.Extensions.IdentityUnitTests
{
    public class UserStoreTests
    {
        [Fact]
        public async Task Test_Null_Parameters()
        {
            var mockBucket = new Mock<IBucket>();
         // var context = new BucketContext(mockBucket.Object);
            var mockBucketProvider = new Mock<ICouchbaseIdentityBucketProvider>();
            mockBucketProvider.Setup(x => x.GetBucket()).Returns(mockBucket.Object);

#pragma warning disable Await1 // Method is not configured to be awaited
            Assert.Throws<ArgumentNullException>("provider", () => new UserStore(null));
            var store = new UserStore(mockBucketProvider.Object);

            //IUserPasswordStore
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.GetUserIdAsync(null, CancellationToken.None));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.GetUserNameAsync(null, CancellationToken.None));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.SetUserNameAsync(null, null, CancellationToken.None));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.CreateAsync(null, CancellationToken.None));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.UpdateAsync(null, CancellationToken.None));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.DeleteAsync(null, CancellationToken.None));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.GetPasswordHashAsync(null, CancellationToken.None));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.SetPasswordHashAsync(null, null, CancellationToken.None));

/*            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.AddClaimsAsync(null, null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.ReplaceClaimAsync(null, null, null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.RemoveClaimsAsync(null, null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.GetClaimsAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.GetLoginsAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.GetRolesAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.AddLoginAsync(null, null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.RemoveLoginAsync(null, null, null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.AddToRoleAsync(null, null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.RemoveFromRoleAsync(null, null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.IsInRoleAsync(null, null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.GetSecurityStampAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.SetSecurityStampAsync(null, null));
            await Assert.ThrowsAsync<ArgumentNullException>("login", async () => await store.AddLoginAsync(new IdentityUser("fake"), null));
            await Assert.ThrowsAsync<ArgumentNullException>("claims",  async () => await store.AddClaimsAsync(new IdentityUser("fake"), null));
            await Assert.ThrowsAsync<ArgumentNullException>("claims", async () => await store.RemoveClaimsAsync(new IdentityUser("fake"), null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.GetEmailConfirmedAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.SetEmailConfirmedAsync(null, true));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.GetEmailAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.SetEmailAsync(null, null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.GetPhoneNumberAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.SetPhoneNumberAsync(null, null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.GetPhoneNumberConfirmedAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user",  async () => await store.SetPhoneNumberConfirmedAsync(null, true));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.GetTwoFactorEnabledAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user",  async () => await store.SetTwoFactorEnabledAsync(null, true));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.GetAccessFailedCountAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.GetLockoutEnabledAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.SetLockoutEnabledAsync(null, false));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.GetLockoutEndDateAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.SetLockoutEndDateAsync(null, new DateTimeOffset()));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.ResetAccessFailedCountAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", async () => await store.IncrementAccessFailedCountAsync(null));
            await Assert.ThrowsAsync<ArgumentException>("normalizedRoleName", async () => await store.AddToRoleAsync(new IdentityUser("fake"), null));
            await Assert.ThrowsAsync<ArgumentException>("normalizedRoleName", async () => await store.RemoveFromRoleAsync(new IdentityUser("fake"), null));
            await Assert.ThrowsAsync<ArgumentException>("normalizedRoleName", async () => await store.IsInRoleAsync(new IdentityUser("fake"), null));
            await Assert.ThrowsAsync<ArgumentException>("normalizedRoleName", async () => await store.AddToRoleAsync(new IdentityUser("fake"), ""));
            await Assert.ThrowsAsync<ArgumentException>("normalizedRoleName", async () => await store.RemoveFromRoleAsync(new IdentityUser("fake"), ""));
            await Assert.ThrowsAsync<ArgumentException>("normalizedRoleName", async () => await store.IsInRoleAsync(new IdentityUser("fake"), ""));
            */
#pragma warning restore Await1 // Method is not configured to be awaited
        }

        [Fact]
        public async void Test_GetClaimsAsync()
        {
            var mockBucket = new Mock<IBucket>();
            var mockBucketProvider = new Mock<ICouchbaseIdentityBucketProvider>();
            mockBucketProvider.Setup(x => x.GetBucket()).Returns(mockBucket.Object);

            var store = new UserStore<IdentityUser>(mockBucketProvider.Object);
            var claims = await store.GetClaimsAsync(new IdentityUser
            {
                Claims = new List<IdentityUserClaim>
                {
                    new IdentityUserClaim(new Claim("theclaim", "myclaim"))
                }
            }, new CancellationToken(false)).ConfigureAwait(false);

            Assert.IsType<List<Claim>>(claims);
        }

        [Fact]
        public async void Test_ReplaceClaimAsync()
        {
            var mockBucket = new Mock<IBucket>();
            var mockBucketProvider = new Mock<ICouchbaseIdentityBucketProvider>();
            mockBucketProvider.Setup(x => x.GetBucket()).Returns(mockBucket.Object);

            var store = new UserStore<IdentityUser>(mockBucketProvider.Object);
            var user = new IdentityUser
            {
                Claims = new List<IdentityUserClaim>
                {
                    new IdentityUserClaim(new Claim("theclaim1", "myclaim")),
                    new IdentityUserClaim(new Claim("theclaim2", "myclaim")),
                    new IdentityUserClaim(new Claim("theclaim3", "myclaim"))
                }
            };

            var newClaim = new IdentityUserClaim(new Claim("newclaim", "newvalue"));

            await store.ReplaceClaimAsync(user,
                user.Claims[1].ToSecurityClaim(),
                newClaim.ToSecurityClaim(),
                new CancellationToken(false)).ConfigureAwait(false);

            Assert.Equal(newClaim, user.Claims[1]);
        }

        [Fact]
        public async void Test_RemoveClaimAsync()
        {
            var mockBucket = new Mock<IBucket>();
            var mockBucketProvider = new Mock<ICouchbaseIdentityBucketProvider>();
            mockBucketProvider.Setup(x => x.GetBucket()).Returns(mockBucket.Object);

            var store = new UserStore<IdentityUser>(mockBucketProvider.Object);
            var user = new IdentityUser
            {
                Claims = new List<IdentityUserClaim>
                {
                    new IdentityUserClaim(new Claim("theclaim1", "myclaim")),
                    new IdentityUserClaim(new Claim("theclaim2", "myclaim")),
                    new IdentityUserClaim(new Claim("theclaim3", "myclaim")),
                    new IdentityUserClaim(new Claim("theclaim4", "myclaim"))
                }
            };

            var claimsToRemove = new List<Claim>
            {
                new Claim("theclaim2", "myclaim"),
                new Claim("theclaim3", "myclaim"),
            };

            await store.RemoveClaimsAsync(user, claimsToRemove, new CancellationToken(false));

            Assert.True(user.Claims.Count == 2);
            Assert.True(user.Claims[0].Type == "theclaim1");
            Assert.True(user.Claims[1].Type == "theclaim4");
        }
    }
}
