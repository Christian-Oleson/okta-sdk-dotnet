﻿// <copyright file="IRequestExecutor.cs" company="Okta, Inc">
// Copyright (c) 2014-2017 Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Threading;
using System.Threading.Tasks;

namespace Okta.Sdk.Abstractions
{
    public interface IRequestExecutor
    {
        Task<HttpResponse<string>> GetAsync(string href, CancellationToken cancellationToken);

        Task<HttpResponse<string>> PostAsync(string href, string body, CancellationToken cancellationToken);
    }
}
