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
using System.Text;

namespace Ubiety.Scram.Core.Attributes
{
    /// <summary>
    /// Represents a SCRAM 'Channel' attribute, used to specify a channel binding type during SCRAM authentication.
    /// </summary>
    /// <remarks>
    /// The channel binding information is encoded as a base64 string when creating the attribute.
    /// </remarks>
    public class ChannelAttribute : ScramAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ChannelAttribute"/> class.
        /// </summary>
        /// <param name="header">String representation of the GS2 header.</param>
        /// <param name="token">Channel binding token.</param>
        public ChannelAttribute(string header, byte[]? token = null)
            : base(ChannelName)
        {
            Header = header;
            Token = token;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelAttribute"/> class.
        /// </summary>
        public ChannelAttribute()
            : base(ChannelName)
        {
            Header = string.Empty;
            Token = null;
        }

        /// <summary>
        /// Gets the string representation of the GS2 header.
        /// </summary>
        public string Header { get; }

        /// <summary>
        /// Gets the token representing the channel for binding.
        /// </summary>
        public byte[]? Token { get; }

        /// <summary>
        /// Implicitly converts a <see cref="ChannelAttribute"/> to a string.
        /// </summary>
        /// <param name="attribute">Attribute to convert.</param>
        /// <returns>String representation of the <see cref="ChannelAttribute"/>.</returns>
        public static implicit operator string(ChannelAttribute attribute) => attribute.ToString();

        /// <summary>
        /// Converts instance to a string.
        /// </summary>
        /// <returns>String representation of the attribute.</returns>
        public override string ToString()
        {
            var attribute = (Token is null)
                ? Encoding.UTF8.GetBytes(Header)
                : (byte[])Encoding.UTF8.GetBytes($"{Header},,").Concat(Token);

            return Convert.ToBase64String(attribute);
        }
    }
}
