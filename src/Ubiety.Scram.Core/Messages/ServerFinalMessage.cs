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
    ///     Final server message.
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
        ///     Gets the server signature.
        /// </summary>
        public ServerSignatureAttribute ServerSignature { get; private set; }

        /// <summary>
        ///     Parse the final server message.
        /// </summary>
        /// <param name="message">Message to parse.</param>
        /// <returns><see cref="ServerFinalMessage"/> instance of the message.</returns>
        public static ServerFinalMessage Parse(string message)
        {
            if (!TryParse(message, out var finalMessage))
            {
                throw new MessageParseException();
            }

            return finalMessage;
        }

        /// <summary>
        ///     Try and parse the final server message.
        /// </summary>
        /// <param name="message">Message to parse.</param>
        /// <param name="finalMessage"><see cref="ServerFinalMessage"/> instance of the message.</param>
        /// <returns>true if the parsing succeeded; otherwise false.</returns>
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
