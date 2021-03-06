﻿using System;
using DotNetApis.Logic.Messages;

namespace FunctionApp.Messages
{
    /// <summary>
    /// The message returned from the HTTP trigger function if it has queued a <see cref="GenerateRequestMessage"/>.
    /// </summary>
    public sealed class GenerateRequestQueuedResponseMessage : MessageBaseWithLog
    {
        /// <summary>
        /// The id of the package, lowercased.
        /// </summary>
        public string NormalizedPackageId { get; set; }

        /// <summary>
        /// The version of the package in a normalized form.
        /// </summary>
        public string NormalizedPackageVersion { get; set; }

        /// <summary>
        /// The standard NuGet short name for the target framework, lowercased.
        /// </summary>
        public string NormalizedFrameworkTarget { get; set; }
    }
}
