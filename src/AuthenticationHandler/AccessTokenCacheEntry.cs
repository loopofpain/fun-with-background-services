using System;

namespace TestService.AuthenticationHandler
{
    public class AccessTokenCacheEntry
    {
        public TokenResponse Token { get; }
        public bool IsValid => DateTime.UtcNow < this.RefreshAfterDateUtc;
        private readonly DateTimeOffset RefreshAfterDateUtc;
        public AccessTokenCacheEntry(TokenResponse token)
        {
            this.Token = token;
            this.RefreshAfterDateUtc = DateTimeOffset.UtcNow + TimeSpan.FromSeconds(token.ExpirationInSeconds / 2.0);
        }
    }
}