// This is free and unencumbered software released into the public domain.
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
    /// Represents the SCRAM (Salted Challenge Response Authentication Mechanism) client proof attribute.
    /// </summary>
    /// <remarks>
    /// The client proof attribute is used to verify the client's identity in the SCRAM authentication process.
    /// </remarks>
    public class ClientProofAttribute : ScramAttribute<byte[]>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ClientProofAttribute"/> class.
        /// </summary>
        /// <param name="value">String value of the client proof.</param>
        public ClientProofAttribute(string value)
            : base(ClientProofName, Convert.FromBase64String(value))
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ClientProofAttribute"/> class.
        /// </summary>
        /// <param name="value">Byte array value of the client proof.</param>
        public ClientProofAttribute(byte[] value)
            : base(ClientProofName, value)
        {
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{Name}={Convert.ToBase64String(Value)}";
        }
    }
}
