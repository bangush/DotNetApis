﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp.Messages
{
    /// <summary>
    /// The response message returned by the HTTP trigger function if it is redirecting the client to the documentation JSON location.
    /// </summary>
    public sealed class RedirectResponseMessage : ResponseMessageBase
    {
    }
}