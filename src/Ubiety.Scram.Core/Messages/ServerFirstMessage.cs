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
    /// Represents the first message sent by the server in the SCRAM (Salted Challenge Response Authentication Mechanism) authentication process.
    /// </summary>
    public class ServerFirstMessage
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ServerFirstMessage"/> class.
        /// </summary>
        /// <param name="iterations">Iterations to use when hashing the password.</param>
        /// <param name="nonce">String value of the server nonce.</param>
        /// <param name="salt">Byte array of the password salt.</param>
        public ServerFirstMessage(int iterations, string nonce, byte[] salt)
        {
            Iterations = new IterationsAttribute(iterations);
            Nonce = new NonceAttribute(nonce);
            Salt = new SaltAttribute(salt);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServerFirstMessage"/> class.
        /// </summary>
        /// <param name="iterations">Iterations to use when hashing the password.</param>
        /// <param name="nonce">String value of the server nonce.</param>
        /// <param name="salt">String value of the password salt.</param>
        public ServerFirstMessage(int iterations, string nonce, string salt)
        {
            Iterations = new IterationsAttribute(iterations);
            Nonce = new NonceAttribute(nonce);
            Salt = new SaltAttribute(salt);
        }

        private ServerFirstMessage()
        {
        }

        /// <summary>
        /// Gets the number of iterations for the SCRAM authentication process.
        /// </summary>
        public IterationsAttribute? Iterations { get; private set; }

        /// <summary>
        /// Gets the nonce used in the SCRAM authentication process.
        /// </summary>
        public NonceAttribute? Nonce { get; private set; }

        /// <summary>
        /// Gets the salt value used in the SCRAM authentication process.
        /// </summary>
        public SaltAttribute? Salt { get; private set; }

        /// <summary>
        /// Parses the given SCRAM server first message string and returns a <see cref="ServerFirstMessage"/> instance.
        /// </summary>
        /// <param name="message">The SCRAM server first message as a string to be parsed.</param>
        /// <returns>An instance of <see cref="ServerFirstMessage"/> created from the parsed message.</returns>
        /// <exception cref="MessageParseException">Thrown when the message cannot be parsed.</exception>
        public static ServerFirstMessage Parse(string message)
        {
            if (!TryParse(message, out var firstMessage))
            {
                throw new MessageParseException();
            }

            return firstMessage;
        }

        /// <summary>
        /// Attempts to parse the specified message into a <see cref="ServerFirstMessage"/> instance.
        /// </summary>
        /// <param name="message">The message to parse.</param>
        /// <param name="firstMessage">The resulting <see cref="ServerFirstMessage"/> instance if parsing is successful.</param>
        /// <returns>true if the message is successfully parsed; otherwise, false.</returns>
        public static bool TryParse(string message, out ServerFirstMessage firstMessage)
        {
            firstMessage = new ServerFirstMessage();

            try
            {
                var attributes = ScramAttribute.ParseAll(message);

                if (!attributes.OfType<IterationsAttribute>().Any()
                    || !attributes.OfType<NonceAttribute>().Any()
                    || !attributes.OfType<SaltAttribute>().Any())
                {
                    return false;
                }

                foreach (var attribute in attributes)
                {
                    switch (attribute)
                    {
                        case IterationsAttribute a:
                            firstMessage.Iterations = a;
                            break;
                        case NonceAttribute a:
                            firstMessage.Nonce = a;
                            break;
                        case SaltAttribute a:
                            firstMessage.Salt = a;
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
