using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http.Json;
using Newtonsoft.Json;
using System.Text;

namespace TheWindowsService.Test2.AuthenticationHandler
{
    public class AuthenticationDelegatingHandler : DelegatingHandler
    {
        private const string TokenEndpoint = "connect/token";

        private readonly ApiCredentials _clientCredentials;
        private readonly HttpClient _accessControlHttpClient;
        private readonly AccessTokensCacheManager _accessTokensCacheManager;

        public AuthenticationDelegatingHandler(
            AccessTokensCacheManager accessTokensCacheManager,
            ApiCredentials clientCredentials,
            string identityAuthority)
            : this(accessTokensCacheManager,clientCredentials,new HttpClient { BaseAddress = new Uri(identityAuthority) })
        {
        }

        public AuthenticationDelegatingHandler(
            AccessTokensCacheManager accessTokensCacheManager,
            ApiCredentials clientCredentials,
            HttpClient accessControlHttpClient)
        {
            _accessTokensCacheManager = accessTokensCacheManager;
            _clientCredentials = clientCredentials;
            _accessControlHttpClient = accessControlHttpClient;

            if (_accessControlHttpClient.BaseAddress == null)
            {
                throw new AuthenticationHandlerException($"{nameof(HttpClient.BaseAddress)} should be set to Identity Server url");
            }

            if (_accessControlHttpClient.BaseAddress?.AbsoluteUri.EndsWith("/") == false)
            {
                _accessControlHttpClient.BaseAddress = new Uri(_accessControlHttpClient.BaseAddress.AbsoluteUri + "/");
            }
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await GetToken();

            request.Headers.Authorization = new AuthenticationHeaderValue(token.Scheme, token.AccessToken);

            return await base.SendAsync(request, cancellationToken);
        }

        private async Task<TokenResponse> GetToken()
        {
            var token = _accessTokensCacheManager.GetToken();
            if (token == null)
            {
                token = await GetNewToken(_clientCredentials);
                _accessTokensCacheManager.SetToken(token);
            }

            return token;
        }

        private async Task<TokenResponse> GetNewToken(ApiCredentials credentials)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, TokenEndpoint))
            {
                request.Content = new StringContent(JsonConvert.SerializeObject(credentials), Encoding.UTF8, "application/json");

                var response = await _accessControlHttpClient.SendAsync(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var tokenResponse = await response.DeserializeAsync<TokenResponse>();
                    return tokenResponse;
                }

                var errorMessage = await GetErrorMessageAsync(response);
                throw new AuthenticationHandlerException(errorMessage);
            }
        }

        private async Task<string> GetErrorMessageAsync(HttpResponseMessage response)
        {
            var baseErrorMessage =
                $"Error occured while trying to get access token from identity authority {response.RequestMessage.RequestUri}.";

            var errorMessage = baseErrorMessage;

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorResponse = await response.DeserializeAsync<TokenErrorResponse>();
                errorMessage = $"{errorMessage} Error details: {errorResponse.Error}";
            }
            else
            {
                errorMessage = $"{errorMessage} Status code: {(int)response.StatusCode} - {response.StatusCode}";
            }

            return errorMessage;
        }
    }
}