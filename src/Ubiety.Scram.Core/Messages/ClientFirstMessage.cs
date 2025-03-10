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
using JetBrains.Annotations;
using Ubiety.Scram.Core.Attributes;
using Ubiety.Scram.Core.Exceptions;

namespace Ubiety.Scram.Core.Messages
{
    /// <summary>
    /// Represents the first message sent by the client in the SCRAM authentication process.
    /// </summary>
    public class ClientFirstMessage
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ClientFirstMessage"/> class.
        /// </summary>
        /// <param name="username">Username of the user to authenticate.</param>
        /// <param name="nonce">String value of the client nonce.</param>
        /// <param name="bindingStatus">Binding status of the message.</param>
        public ClientFirstMessage(string username, string nonce, ChannelBindingStatus bindingStatus = ChannelBindingStatus.NotSupported)
        {
            Username = new UserAttribute(username);
            Nonce = new NonceAttribute(nonce);
            Gs2Header = new Gs2Attribute { ChannelBindingStatus = bindingStatus };
        }

        private ClientFirstMessage()
        {
            Gs2Header = new Gs2Attribute { ChannelBindingStatus = ChannelBindingStatus.NotSupported };
        }

        /// <summary>
        /// Gets the GS2 header attribute of the message, which represents the channel binding status
        /// as part of the SCRAM (Salted Challenge Response Authentication Mechanism) protocol.
        /// </summary>
        public Gs2Attribute Gs2Header { get; private set; }

        /// <summary>
        /// Gets the username attribute of the client, used in the SCRAM authentication process.
        /// This property represents the user's identifier with specific formatting and escaping rules.
        /// </summary>
        public UserAttribute? Username { get; private set; }

        /// <summary>
        /// Gets the nonce attribute used in the client's first message during the SCRAM authentication process.
        /// The nonce is a unique, randomly generated value used to ensure the integrity and security of the authentication exchange.
        /// </summary>
        public NonceAttribute? Nonce { get; private set; }

        /// <summary>
        /// Gets the bare version of the client message, concatenating the username and nonce attributes
        /// as formatted strings for use in the SCRAM authentication process.
        /// </summary>
        public string BareMessage => $"{Username},{Nonce}";

        /// <summary>
        /// Gets the complete SCRAM client-first-message string.
        /// This property combines various SCRAM attributes, including GS2 header,
        /// username, and nonce, to form the full authentication message.
        /// </summary>
        public string Message => $"{Gs2Header}{BareMessage}";

        /// <summary>
        /// Parses the provided message into an instance of <see cref="ClientFirstMessage"/>.
        /// </summary>
        /// <param name="message">Message to parse.</param>
        /// <returns>An instance of <see cref="ClientFirstMessage"/> created from the provided message.</returns>
        [UsedImplicitly]
        public static ClientFirstMessage Parse(string message)
        {
            if (!TryParse(message, out var result))
            {
                throw new MessageParseException();
            }

            return result;
        }

        /// <summary>
        /// Attempts to parse the provided message into an instance of <see cref="ClientFirstMessage"/>.
        /// </summary>
        /// <param name="message">The SCRAM message to be parsed.</param>
        /// <param name="result">The resulting <see cref="ClientFirstMessage"/> if parsing succeeds; otherwise null.</param>
        /// <returns>true if the message is successfully parsed; otherwise false.</returns>
        [UsedImplicitly]
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
