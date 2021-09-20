using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace TestService.AuthenticationHandler
{
    public static class HttpClientBuilderExtensions
    {
        public static IHttpClientBuilder AddAuthentication(this IHttpClientBuilder builder,
            Func<IServiceProvider, ApiCredentials> credentialsProvider,
            Func<IServiceProvider, string> identityAuthorityProvider)
        {
            builder.Services.TryAddSingleton<AccessTokensCacheManager>();
            builder.AddHttpMessageHandler(provider =>
            {
                var credentials = credentialsProvider.Invoke(provider);
                var identityAuthority = identityAuthorityProvider.Invoke(provider);

                return CreateDelegatingHandler(provider, credentials, identityAuthority);
            });

            return builder;
        }



        public static IHttpClientBuilder AddAuthentication(this IHttpClientBuilder builder,
   ApiCredentials credentialsProvider,
    string identityAuthorityProvider)
        {
            builder.Services.TryAddSingleton<AccessTokensCacheManager>();
            builder.AddHttpMessageHandler(provider => CreateDelegatingHandler(provider, credentialsProvider, identityAuthorityProvider));

            return builder;
        }

        private static AuthenticationDelegatingHandler CreateDelegatingHandler(IServiceProvider provider,
            ApiCredentials credentials, string identityAuthority)
        {
            var httpClient = CreateHttpClient(provider, identityAuthority);
            var accessTokensCacheManager = provider.GetRequiredService<AccessTokensCacheManager>();

            return new AuthenticationDelegatingHandler(accessTokensCacheManager, credentials, httpClient);
        }

        private static HttpClient CreateHttpClient(IServiceProvider provider, string identityAuthority)
        {
            var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(identityAuthority);

            return httpClient;
        }
    }
}