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
using System.Text;
using Ubiety.Scram.Core.Attributes;

namespace Ubiety.Scram.Core.Messages
{
    /// <summary>
    /// Represents the final message sent from the client during the SCRAM (Salted Challenge Response Authentication Mechanism) authentication process.
    /// </summary>
    public class ClientFinalMessage
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ClientFinalMessage"/> class.
        /// </summary>
        /// <param name="clientFirstMessage">First client message.</param>
        /// <param name="serverFirstMessage">First server message.</param>
        /// <param name="password">User password.</param>
        /// <param name="hash"><see cref="Hash"/> to use when calculating proof.</param>
        /// <param name="token">Token to use for channel binding.</param>
        public ClientFinalMessage(ClientFirstMessage clientFirstMessage, ServerFirstMessage serverFirstMessage, string password, Hash hash, byte[]? token = null)
        {
            Channel = new ChannelAttribute(clientFirstMessage.Gs2Header, token);
            Nonce = new NonceAttribute(serverFirstMessage.Nonce?.Value);

            CalculateProof(password, hash, clientFirstMessage, serverFirstMessage);
        }

        /// <summary>
        /// Gets the channel attribute used for encoding the GS2 (Generic Security Service) header in the SCRAM authentication process.
        /// </summary>
        public ChannelAttribute Channel { get; }

        /// <summary>
        /// Gets the nonce attribute used during the SCRAM (Salted Challenge Response Authentication Mechanism) process to ensure message uniqueness and mitigate replay attacks.
        /// </summary>
        public NonceAttribute Nonce { get; }

        /// <summary>
        /// Gets the proof attribute, which represents the computed client proof used in the SCRAM authentication process.
        /// </summary>
        public ClientProofAttribute? Proof { get; private set; }

        /// <summary>
        /// Gets the constructed client final message content excluding the proof value,
        /// which includes the channel binding information and the nonce.
        /// </summary>
        public string MessageWithoutProof => $"{Channel},{Nonce}";

        /// <summary>
        /// Gets the complete message for the client in the final stage of SCRAM authentication,
        /// including the channel binding, nonce, and optionally the client proof, formatted as a single string.
        /// </summary>
        public string Message => $"{MessageWithoutProof},{Proof}";

        /// <summary>
        /// Gets the server signature as calculated with the proof.
        /// </summary>
        public string ServerSignature { get; internal set; } = string.Empty;

        /// <summary>
        /// Implicitly converts a <see cref="ClientFinalMessage"/> to its byte array representation using UTF-8 encoding.
        /// </summary>
        /// <param name="message">The client final message to convert.</param>
        /// <returns>A byte array containing the UTF-8 encoded message.</returns>
        public static implicit operator byte[](ClientFinalMessage message) => Encoding.UTF8.GetBytes(message.Message);

        private void CalculateProof(string password, Hash hash, ClientFirstMessage clientFirstMessage, ServerFirstMessage serverFirstMessage)
        {
            var preppedPassword = SaslPrep.Run(password);
            var saltedPassword = hash.ComputeHash(Encoding.UTF8.GetBytes(preppedPassword), serverFirstMessage.Salt?.Value ?? throw new InvalidOperationException(), serverFirstMessage.Iterations?.Value ?? throw new InvalidOperationException());

            var clientKey = hash.ComputeHash("Client Key"u8.ToArray(), saltedPassword);
            var serverKey = hash.ComputeHash("Server Key"u8.ToArray(), saltedPassword);
            var storedKey = hash.ComputeHash(clientKey);

            var authMessage = $"{clientFirstMessage.BareMessage},{serverFirstMessage.Message},{MessageWithoutProof}";
            var auth = Encoding.UTF8.GetBytes(authMessage);

            var signature = hash.ComputeHash(auth, storedKey);
            ServerSignature = Encoding.UTF8.GetString(hash.ComputeHash(auth, serverKey));

            Proof = new ClientProofAttribute(clientKey.ExclusiveOr(signature));
        }
    }
}
