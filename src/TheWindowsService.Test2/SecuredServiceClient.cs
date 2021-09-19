using System.Net.Http;
using System.Threading.Tasks;

namespace TheWindowsService.Test2
{
    public class ApiServiceClient
    {
        private readonly HttpClient _client;

        public ApiServiceClient(HttpClient client)
        {
            _client = client;
        }

        public Task<HttpResponseMessage> SendAsync()
        {
            return _client.GetAsync("/");
        }
    }
}