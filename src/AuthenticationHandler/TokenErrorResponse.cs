

using Newtonsoft.Json;

namespace TestService.AuthenticationHandler
{
    public class TokenErrorResponse
    {
        [JsonProperty(PropertyName = "error")]
        public string Error { get; set; }
    }
}