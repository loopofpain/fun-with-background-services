

using Newtonsoft.Json;

namespace TheWindowsService.Test2.AuthenticationHandler
{
    public class TokenErrorResponse
    {
        [JsonProperty(PropertyName = "error")]
        public string Error { get; set; }
    }
}