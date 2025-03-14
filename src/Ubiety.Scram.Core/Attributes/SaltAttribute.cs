﻿// This is free and unencumbered software released into the public domain.
//
// Anyone is free to copy, modify, publish, use, compile, sell, or
// distribute this software, either in source code form or as a compiled
// binary, for any purpose, commercial or non-commercial, and by any
// means.
//
// In jurisdictions that recognize copyright laws, the author or authors
// of this software dedicate any and all copyright interest in the
// software to the public domain. We make this dedication for the benefit
// of the public at large and to the detriment of our heirs and
// successors. We intend this dedication to be an overt act of
// relinquishment in perpetuity of all present and future rights to this
// software under copyright law.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// For more information, please refer to <http://unlicense.org/>

using System;

namespace Ubiety.Scram.Core.Attributes
{
    /// <summary>
    /// Represents the SCRAM attribute for the salt used in authentication procedures.
    /// </summary>
    /// <remarks>
    /// The <c>SaltAttribute</c> is a specialized SCRAM attribute that holds the salt value,
    /// which can be provided as either a byte array or a Base64-encoded string.
    /// </remarks>
    public class SaltAttribute : ScramAttribute<byte[]>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SaltAttribute"/> class.
        /// </summary>
        /// <param name="value">Byte array value of the salt.</param>
        public SaltAttribute(byte[] value)
            : base(SaltName, value)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SaltAttribute"/> class.
        /// </summary>
        /// <param name="value">String value of the salt.</param>
        public SaltAttribute(string value)
            : base(SaltName, Convert.FromBase64String(value))
        {
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Name} = {Convert.ToBase64String(Value)}";
        }
    }
}
