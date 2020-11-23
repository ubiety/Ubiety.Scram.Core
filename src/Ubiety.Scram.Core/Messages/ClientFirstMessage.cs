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
using Ubiety.Scram.Core.Attributes;
using Ubiety.Scram.Core.Exceptions;

namespace Ubiety.Scram.Core.Messages
{
    /// <summary>
    ///     First client message.
    /// </summary>
    public class ClientFirstMessage
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ClientFirstMessage"/> class.
        /// </summary>
        /// <param name="username">Username of the user to authenticate.</param>
        /// <param name="nonce">String value of the client nonce.</param>
        public ClientFirstMessage(string username, string nonce)
        {
            Username = new UserAttribute(username);
            Nonce = new NonceAttribute(nonce);
        }

        private ClientFirstMessage()
        {
        }

        /// <summary>
        ///     Gets the GS2 header for the message.
        /// </summary>
        public Gs2Attribute Gs2Header { get; private set; } = new (ChannelBindingStatus.NotSupported);

        /// <summary>
        ///     Gets the username of the message.
        /// </summary>
        public UserAttribute? Username { get; private set; }

        /// <summary>
        ///     Gets the nonce of the message.
        /// </summary>
        public NonceAttribute? Nonce { get; private set; }

        /// <summary>
        ///     Gets the bare client message.
        /// </summary>
        public string BareMessage => $"{Username},{Nonce}";

        /// <summary>
        ///     Gets the client message with the GS2 header.
        /// </summary>
        public string Message => $"{Gs2Header}{BareMessage}";

        /// <summary>
        ///     Parse the first client message.
        /// </summary>
        /// <param name="message">Message to parse.</param>
        /// <returns><see cref="ClientFirstMessage"/> instance of the message.</returns>
        public static ClientFirstMessage Parse(string message)
        {
            if (!TryParse(message, out var result))
            {
                throw new MessageParseException();
            }

            return result;
        }

        /// <summary>
        ///     Try to parse the first client message.
        /// </summary>
        /// <param name="message">Message to parse.</param>
        /// <param name="result"><see cref="ClientFirstMessage"/> instance of the message.</param>
        /// <returns>true if the parsing succeeds; otherwise false.</returns>
        public static bool TryParse(string message, out ClientFirstMessage result)
        {
            result = new ClientFirstMessage();

            try
            {
                var attributes = ScramAttribute.ParseAll(message);

                if (!attributes.OfType<Gs2Attribute>().Any()
                    || !attributes.OfType<UserAttribute>().Any()
                    || !attributes.OfType<NonceAttribute>().Any())
                {
                    return false;
                }

                foreach (var attribute in attributes)
                {
                    switch (attribute)
                    {
                        case Gs2Attribute a:
                            result.Gs2Header = a;
                            break;
                        case UserAttribute a:
                            result.Username = a;
                            break;
                        case NonceAttribute a:
                            result.Nonce = a;
                            break;
                    }
                }
            }
            catch (FormatException)
            {
                return false;
            }

            return true;
        }
    }
}
