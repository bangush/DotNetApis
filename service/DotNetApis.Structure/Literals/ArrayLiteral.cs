﻿using System.Collections.Generic;
using DotNetApis.Structure.TypeReferences;
using Newtonsoft.Json;

namespace DotNetApis.Structure.Literals
{
    /// <summary>
    /// A constant array.
    /// </summary>
    public sealed class ArrayLiteral : ILiteral
    {
        public LiteralKind Kind => LiteralKind.Array;

        /// <summary>
        /// The type of the array elements.
        /// </summary>
        [JsonProperty("t")]
        public ITypeReference ElementType { get; set; }

        /// <summary>
        /// The array element values.
        /// </summary>
        [JsonProperty("v")]
        public IReadOnlyList<ILiteral> Values { get; set; }

        public override string ToString() => "[" + string.Join(",", Values) + "]";
    }
}
