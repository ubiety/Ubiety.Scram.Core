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
        /// <param name="token">Token to use for channel binding.</param>
        public ClientFinalMessage(ClientFirstMessage clientFirstMessage, ServerFirstMessage serverFirstMessage, byte[]? token = null)
        {
            Channel = new ChannelAttribute(clientFirstMessage.Gs2Header, token);
            Nonce = new NonceAttribute(serverFirstMessage.Nonce?.Value);
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
        /// Sets the client proof attribute with the provided byte array.
        /// </summary>
        /// <param name="proof">Byte array of the proof.</param>
        public void SetProof(byte[] proof)
        {
            Proof = new ClientProofAttribute(proof);
        }

        /// <summary>
        /// Sets the proof for the client final message.
        /// </summary>
        /// <param name="proof">The proof as a string value.</param>
        public void SetProof(string proof)
        {
            Proof = new ClientProofAttribute(proof);
        }
    }
}
