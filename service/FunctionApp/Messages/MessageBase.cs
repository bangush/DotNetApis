﻿using System;
using Common;

namespace FunctionApp.Messages
{
    /// <summary>
    /// The base type for all messages generated by the HTTP trigger function.
    /// </summary>
    public abstract class MessageBase
    {
        /// <summary>
        /// The AppInsights operation id.
        /// </summary>
        public Guid OperationId { get; set; } = AmbientContext.OperationId;

        /// <summary>
        /// The HTTP request id.
        /// </summary>
        public string RequestId { get; set; } = AmbientContext.RequestId;
    }
}