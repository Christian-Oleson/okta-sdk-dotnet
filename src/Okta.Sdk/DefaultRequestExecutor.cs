﻿using Microsoft.Extensions.Logging;
using Okta.Sdk.Abstractions;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Okta.Sdk
{
    public sealed class DefaultRequestExecutor : IRequestExecutor
    {
        private const string OktaClientUserAgentName = "oktasdk-dotnet";

        private readonly string _orgUrl;
        private readonly string _defaultUserAgent;
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public DefaultRequestExecutor(string orgUrl, string token, ILogger logger)
        {
            if (string.IsNullOrEmpty(token)) throw new ArgumentNullException(nameof(token));

            _orgUrl = EnsureCorrectOrgUrl(orgUrl);
            _defaultUserAgent = CreateUserAgent();
            _httpClient = CreateClient(_orgUrl, token, _defaultUserAgent);
            _logger = logger;
        }

        private static string EnsureCorrectOrgUrl(string orgUrl)
        {
            if (string.IsNullOrEmpty(orgUrl)) throw new ArgumentNullException(nameof(orgUrl));

            if (!orgUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Org URL must start with https://");
            }

            if (!orgUrl.EndsWith("/"))
            {
                orgUrl += "/";
            }

            return orgUrl;
        }

        // TODO IFrameworkUserAgentBuilder
        private static string CreateUserAgent()
            => $"{OktaClientUserAgentName}/0.0.1"; // todo assembly version

        private static HttpClient CreateClient(string orgBaseUrl, string token, string userAgent)
        {
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = false
            };

            var client = new HttpClient(handler, true)
            {
                BaseAddress = new Uri(orgBaseUrl, UriKind.Absolute)
            };

            client.DefaultRequestHeaders.Add("User-Agent", userAgent);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("SSWS", token);

            // Workaround for https://github.com/dotnet/corefx/issues/11224
            client.DefaultRequestHeaders.Add("Connection", "close");

            return client;
        }

        private void EnsureRequestUrlMatchesOrg(string url)
        {
            if (string.IsNullOrEmpty(url) || !url.StartsWith(_orgUrl, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Resource URL must begin with Organization URL.");
            }
        }

        private string MakeUrlRelative(string url)
            => url.Replace(_orgUrl, string.Empty);

        public Task<HttpResponse<string>> GetAsync(string href, CancellationToken cancellationToken)
        {
            EnsureRequestUrlMatchesOrg(href);
            var path = MakeUrlRelative(href);

            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(path, UriKind.Relative));
            return SendAsync(request, cancellationToken);
        }

        public async Task<string> GetBodyAsync(string href, CancellationToken cancellationToken)
        {
            var response = await GetAsync(href, cancellationToken).ConfigureAwait(false);
            return response.Payload;
        }

        public Task<HttpResponse<string>> PostAsync(string href, string body, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private async Task<HttpResponse<string>> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            _logger.LogTrace($"{request.Method} {request.RequestUri}");

            using (var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
            {
                _logger.LogTrace($"{(int)response.StatusCode} {request.RequestUri.PathAndQuery}");

                string stringContent = null;
                if (response.Content != null)
                {
                    stringContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }

                return new HttpResponse<string>
                {
                    Headers = ExtractHeaders(response),
                    StatusCode = (int)response.StatusCode,
                    Payload = stringContent
                };
            }
        }

        private IEnumerable<KeyValuePair<string, IEnumerable<string>>> ExtractHeaders(HttpResponseMessage response)
            => response.Headers.Concat(response.Content.Headers);
    }
}
