﻿// <copyright file="OktaClient.cs" company="Okta, Inc">
// Copyright (c) 2014-2017 Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Okta.Sdk.Abstractions;

namespace Okta.Sdk
{
    public partial class OktaClient : IOktaClient
    {
        private readonly ApiClientConfiguration _configuration;
        private readonly string _orgUriWithApiPrefix;
        private readonly ILogger _logger;

        public OktaClient(ApiClientConfiguration apiClientConfiguration = null, ILogger logger = null)
        {
            // TODO: flexible configuration

            //string configurationFileRoot = null; // TODO find the application root directory at runtime?

            //var homeOktaJsonLocation = HomePath.Resolve("~", ".okta", "okta.json");
            //var homeOktaYamlLocation = HomePath.Resolve("~", ".okta", "okta.yaml");

            //var applicationAppSettingsLocation = Path.Combine(configurationFileRoot ?? string.Empty, "appsettings.json");
            //var applicationOktaJsonLocation = Path.Combine(configurationFileRoot ?? string.Empty, "okta.json");
            //var applicationOktaYamlLocation = Path.Combine(configurationFileRoot ?? string.Empty, "okta.yaml");

            //var configBuilder = new ConfigurationBuilder()
            //    .AddYamlFile(homeOktaYamlLocation, optional: true, root: "okta")
            //    .AddJsonFile(homeOktaJsonLocation, optional: true, root: "okta")
            //    .AddJsonFile(applicationAppSettingsLocation, optional: true)
            //    .AddYamlFile(applicationOktaYamlLocation, optional: true, root: "okta")
            //    .AddJsonFile(applicationOktaJsonLocation, optional: true, root: "okta")
            //    .AddEnvironmentVariables("okta", "_", root: "okta")
            //    .AddObject(apiClientConfiguration, root: "okta");

            //var config = configBuilder.Build();

            // TODO: for now, just doing dumb configuration

            // TODO: validate configuration
            _configuration = apiClientConfiguration;

            _orgUriWithApiPrefix = _configuration.OrgUrl;
            if (_configuration.OrgUrl.EndsWith("/"))
            {
                _orgUriWithApiPrefix.TrimEnd('/');
            }

            _logger = logger ?? NullLogger.Instance;

            // TODO pass proxy, connectionTimeout, etc
            DataStore = new DefaultDataStore(
                new DefaultRequestExecutor(apiClientConfiguration.OrgUrl, apiClientConfiguration.Token, _logger),
                new DefaultSerializer(),
                _logger);
        }

        public OktaClient(IDataStore dataStore)
        {
            DataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));
        }

        public IDataStore DataStore { get; }

        /// <inheritdoc/>
        public UsersClient Users => new UsersClient(this);

        private string ToAbsoluteUri(string path)
        {
            if (path.StartsWith("http://") || path.StartsWith("https://"))
            {
                return path;
            }

            var pathWithLeadingSlash = path.StartsWith("/")
                ? path
                : $"/{path}";

            return $"{_orgUriWithApiPrefix}{pathWithLeadingSlash}";
        }

        /// <inheritdoc/>
        public Task<T> GetAsync<T>(string href, CancellationToken cancellationToken = default(CancellationToken))
            where T : Resource, new()
            => GetAsync<T>(new HttpRequest { Uri = href }, cancellationToken);

        /// <inheritdoc/>
        public async Task<T> GetAsync<T>(HttpRequest request, CancellationToken cancellationToken = default(CancellationToken))
            where T : Resource, new()
        {
            var response = await DataStore.GetAsync<T>(request, cancellationToken).ConfigureAwait(false);
            return response?.Payload;
        }

        /// <inheritdoc/>
        public Task PostAsync(string href, object model, CancellationToken cancellationToken = default(CancellationToken))
            => PostAsync(new HttpRequest { Uri = href, Payload = model }, cancellationToken);

        /// <inheritdoc/>
        public Task<TResponse> PostAsync<TResponse>(string href, object model, CancellationToken cancellationToken = default(CancellationToken))
            where TResponse : Resource, new()
            => PostAsync<TResponse>(new HttpRequest { Uri = href, Payload = model }, cancellationToken);

        /// <inheritdoc/>
        public Task PostAsync(HttpRequest request, CancellationToken cancellationToken = default(CancellationToken))
            => PostAsync<Resource>(request, cancellationToken);

        /// <inheritdoc/>
        public async Task<TResponse> PostAsync<TResponse>(HttpRequest request, CancellationToken cancellationToken = default(CancellationToken))
            where TResponse : Resource, new()
        {
            var response = await DataStore.PostAsync<TResponse>(request, cancellationToken).ConfigureAwait(false);
            return response?.Payload;
        }

        /// <inheritdoc/>
        public Task PutAsync(string href, object model, CancellationToken cancellationToken = default(CancellationToken))
            => PutAsync(new HttpRequest { Uri = href, Payload = model }, cancellationToken);

        /// <inheritdoc/>
        public Task<TResponse> PutAsync<TResponse>(string href, object model, CancellationToken cancellationToken = default(CancellationToken))
            where TResponse : Resource, new()
            => PutAsync<TResponse>(new HttpRequest { Uri = href, Payload = model }, cancellationToken);

        /// <inheritdoc/>
        public Task PutAsync(HttpRequest request, CancellationToken cancellationToken = default(CancellationToken))
            => PutAsync<Resource>(request, cancellationToken);

        /// <inheritdoc/>
        public async Task<TResponse> PutAsync<TResponse>(HttpRequest request, CancellationToken cancellationToken = default(CancellationToken))
            where TResponse : Resource, new()
        {
            var response = await DataStore.PostAsync<TResponse>(request, cancellationToken).ConfigureAwait(false);
            return response?.Payload;
        }

        /// <inheritdoc/>
        public Task DeleteAsync(string href, CancellationToken cancellationToken = default(CancellationToken))
            => DeleteAsync(new HttpRequest { Uri = href }, cancellationToken);

        /// <inheritdoc/>
        public Task DeleteAsync(HttpRequest request, CancellationToken cancellationToken = default(CancellationToken))
            => DataStore.DeleteAsync(request, cancellationToken);
    }
}
