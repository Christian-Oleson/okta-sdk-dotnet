﻿// <copyright file="OktaApiException.cs" company="Okta, Inc">
// Copyright (c) Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

namespace Okta.Sdk
{
    /// <summary>
    /// Represents an error returned by the Okta API.
    /// </summary>
    public class OktaApiException : OktaException
    {
        private readonly Resource _resource = new Resource();

        /// <summary>
        /// Initializes a new instance of the <see cref="OktaApiException"/> class.
        /// </summary>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="data">The error data.</param>
        public OktaApiException(int statusCode, Resource data)
            : base(message: data.GetProperty<string>(nameof(ErrorSummary)))
        {
            StatusCode = statusCode;
            _resource = data;
        }

        /// <summary>
        /// Gets the HTTP status code.
        /// </summary>
        /// <value>
        /// The HTTP status code.
        /// </value>
        public int StatusCode { get; }

        /// <summary>
        /// Gets the error code from the Okta error details.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        public string ErrorCode => _resource.GetProperty<string>(nameof(ErrorCode));

        /// <summary>
        /// Gets the error summary from the Okta error details.
        /// </summary>
        /// <value>
        /// The error summary.
        /// </value>
        public string ErrorSummary => _resource.GetProperty<string>(nameof(ErrorSummary));

        /// <summary>
        /// Gets the error link from the Okta error details.
        /// </summary>
        /// <value>
        /// The error link.
        /// </value>
        public string ErrorLink => _resource.GetProperty<string>(nameof(ErrorLink));

        /// <summary>
        /// Gets the error ID from the Okta error details.
        /// </summary>
        /// <value>
        /// The error ID.
        /// </value>
        public string ErrorId => _resource.GetProperty<string>(nameof(ErrorId));

        // TODO errorCauses (list of ?)
    }
}
