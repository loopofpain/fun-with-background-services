using Newtonsoft.Json;

namespace TestService.AuthenticationHandler
{
    public class TokenResponse
    {
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }

        [JsonProperty(PropertyName = "scheme")]
        public string Scheme { get; set; }

        [JsonProperty(PropertyName = "expiration_in_seconds")]
        public long ExpirationInSeconds { get; set; }
    }
}