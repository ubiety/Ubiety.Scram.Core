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
    /// Represents the final message sent by the server in the SCRAM authentication process.
    /// </summary>
    public class ServerFinalMessage
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ServerFinalMessage"/> class.
        /// </summary>
        /// <param name="serverSignature">Server signature.</param>
        public ServerFinalMessage(ServerSignatureAttribute serverSignature)
        {
            ServerSignature = serverSignature;
        }

        private ServerFinalMessage()
        {
        }

        /// <summary>
        /// Gets the server's computed signature during the SCRAM authentication process.
        /// </summary>
        /// <remarks>
        /// The <c>ServerSignature</c> ensures the integrity of the authentication handshake
        /// by verifying that the server knows the user's credentials without explicitly exposing them.
        /// </remarks>
        public ServerSignatureAttribute? ServerSignature { get; private set; }

        /// <summary>
        /// Parses a server final message string into a <see cref="ServerFinalMessage"/> object.
        /// </summary>
        /// <param name="message">The server final message string to parse.</param>
        /// <returns>An instance of <see cref="ServerFinalMessage"/> parsed from the provided message.</returns>
        /// <exception cref="MessageParseException">Thrown when the message cannot be parsed.</exception>
        public static ServerFinalMessage Parse(string message)
        {
            if (!TryParse(message, out var finalMessage))
            {
                throw new MessageParseException();
            }

            return finalMessage;
        }

        /// <summary>
        /// Tries to parse a server final message from the given string.
        /// </summary>
        /// <param name="message">The message string to parse.</param>
        /// <param name="finalMessage">The resulting <see cref="ServerFinalMessage"/> instance if parsing is successful.</param>
        /// <returns>true if the parsing is successful; otherwise, false.</returns>
        public static bool TryParse(string message, out ServerFinalMessage finalMessage)
        {
            finalMessage = new ServerFinalMessage();

            try
            {
                var attributes = ScramAttribute.ParseAll(message);

                if (!attributes.OfType<ServerSignatureAttribute>().Any())
                {
                    return false;
                }

                foreach (var attribute in attributes)
                {
                    switch (attribute)
                    {
                        case ServerSignatureAttribute a:
                            finalMessage.ServerSignature = a;
                            break;
                        case ErrorAttribute a:
                            return false;
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
