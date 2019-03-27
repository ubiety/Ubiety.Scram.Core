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
using System.Linq;

namespace Ubiety.Scram.Core.Attributes
{
    /// <summary>
    ///     Server signature part of SCRAM message.
    /// </summary>
    public sealed class ServerSignatureAttribute : ScramAttribute<byte[]>, IEquatable<ServerSignatureAttribute>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ServerSignatureAttribute"/> class.
        /// </summary>
        /// <param name="value">Byte array of the server signature.</param>
        public ServerSignatureAttribute(byte[] value)
            : base(ServerSignatureName, value)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServerSignatureAttribute"/> class.
        /// </summary>
        /// <param name="value">String value of the server signature.</param>
        public ServerSignatureAttribute(string value)
            : base(ServerSignatureName, Convert.FromBase64String(value))
        {
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as ServerSignatureAttribute);
        }

        /// <inheritdoc cref="ScramAttribute"/>
        public bool Equals(byte[] other)
        {
            return Equals(new ServerSignatureAttribute(other));
        }

        /// <inheritdoc cref="ScramAttribute"/>
        public bool Equals(ServerSignatureAttribute other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Value.SequenceEqual(other?.Value ?? throw new InvalidOperationException());
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
