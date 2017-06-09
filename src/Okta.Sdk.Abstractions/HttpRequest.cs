﻿// <copyright file="HttpRequest.cs" company="Okta, Inc">
// Copyright (c) 2014-2017 Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace Okta.Sdk.Abstractions
{
    public class HttpRequest
    {
        public string Path { get; set; }

        public object Payload { get; set; }

        public IEnumerable<KeyValuePair<string, object>> QueryParams { get; set; }

        public IEnumerable<KeyValuePair<string, object>> PathParams { get; set; }
    }
}
