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
    /// Represents a server signature attribute in the SCRAM (Salted Challenge Response Authentication Mechanism) authentication process.
    /// </summary>
    /// <remarks>
    /// This attribute is used to store the server's computed signature and ensures the integrity of the SCRAM handshake.
    /// It is identified by the character 'v' as per the SCRAM specification.
    /// </remarks>
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

        /// <summary>
        /// Compares a <see cref="ServerSignatureAttribute"/> instance with a string representation of a server signature.
        /// </summary>
        /// <param name="left">The <see cref="ServerSignatureAttribute"/> instance to compare.</param>
        /// <param name="right">The string representation of a server signature to compare against.</param>
        /// <returns>
        /// <c>true</c> if the <paramref name="left"/> instance's value matches the base64-decoded value of <paramref name="right"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(ServerSignatureAttribute? left, string right)
        {
            return Equals(left, new ServerSignatureAttribute(right));
        }

        /// <summary>
        /// Compares a <see cref="ServerSignatureAttribute"/> instance with a string representation of a server signature for inequality.
        /// </summary>
        /// <param name="left">The <see cref="ServerSignatureAttribute"/> instance to compare.</param>
        /// <param name="right">The string representation of a server signature to compare against.</param>
        /// <returns>
        /// <c>true</c> if the <paramref name="left"/> instance's value does not match the base64-decoded value of <paramref name="right"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(ServerSignatureAttribute? left, string right)
        {
            return !Equals(left, new ServerSignatureAttribute(right));
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current <see cref="ServerSignatureAttribute"/> instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>
        /// <c>true</c> if the specified object is equal to the current instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object? obj)
        {
            return Equals(obj as ServerSignatureAttribute);
        }

        /// <summary>
        /// Determines whether the current instance is equal to the specified byte array.
        /// </summary>
        /// <param name="other">A byte array to compare with the current instance.</param>
        /// <returns>True if the current instance is equal to the provided byte array; otherwise, false.</returns>
        public bool Equals(byte[] other)
        {
            return Equals(new ServerSignatureAttribute(other));
        }

        /// <summary>
        /// Determines whether the specified <see cref="ServerSignatureAttribute"/> is equal to the current instance.
        /// </summary>
        /// <param name="other">The <see cref="ServerSignatureAttribute"/> to compare with the current instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="ServerSignatureAttribute"/> is equal to the current instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(ServerSignatureAttribute? other)
        {
            return other is not null && (ReferenceEquals(this, other) || Value.SequenceEqual(other.Value));
        }

        /// <summary>
        /// Generates the hash code for the current <see cref="ServerSignatureAttribute"/> instance.
        /// </summary>
        /// <returns>
        /// An integer representing the hash code of the <see cref="ServerSignatureAttribute"/> instance,
        /// which is derived from the attribute name.
        /// </returns>
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
