﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DotNetApis.Structure
{
    /// <summary>
    /// Structured documentation for a fully-qualified type reference. This may be a simple type name or it may be an open generic type.
    /// </summary>
    public sealed class TypeTypeReference : ITypeReference
    {
        public EntityReferenceKind Kind => EntityReferenceKind.Type;

        /// <summary>
        /// The name of the type, without a backtick suffix.
        /// </summary>
        [JsonProperty("n")]
        public string Name { get; set; }

        /// <summary>
        /// The namespace containing this type, if any.
        /// </summary>
        [JsonProperty("s")]
        public string Namespace { get; set; }

        /// <summary>
        /// The type where this type is declared, if any.
        /// </summary>
        [JsonProperty("t")]
        public ITypeReference DeclaringType { get; set; }

        /// <summary>
        /// The location of this type.
        /// </summary>
        [JsonProperty("l")]
        public ILocation Location { get; set; }

        /// <summary>
        /// The number of generic arguments, if this is an open generic type. If this is not an open generic type, this value is <c>0</c>.
        /// </summary>
        [JsonProperty("a")]
        public int GenericArgumentCount { get; set; }
    }
}
