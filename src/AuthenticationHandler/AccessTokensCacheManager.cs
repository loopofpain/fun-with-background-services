namespace TestService.AuthenticationHandler
{
    public class AccessTokensCacheManager
    {
        private AccessTokenCacheEntry token;

        public void SetToken(TokenResponse accessToken)
        {
            this.token = new AccessTokenCacheEntry(accessToken);
        }

        public TokenResponse GetToken()
        {
            if(this.token?.IsValid == true){
                return this.token.Token;
            }

            return null;
        }
    }
}