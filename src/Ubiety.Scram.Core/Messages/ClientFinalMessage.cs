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
        /// The smallest iteration count a server may ask for, as required by RFC 7677 section 4.
        /// </summary>
        /// <remarks>
        /// The iteration count is chosen by the server, so a client that accepts whatever it is
        /// sent lets a hostile or compromised server weaken its own password derivation.
        /// </remarks>
        public const int MinimumIterations = 4096;

        /// <summary>
        /// The largest iteration count this library will derive a key for.
        /// </summary>
        /// <remarks>
        /// Well above anything a real deployment uses, and low enough that a server cannot stall
        /// the client indefinitely by asking for an absurd amount of work.
        /// </remarks>
        public const int MaximumIterations = 10_000_000;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ClientFinalMessage"/> class.
        /// </summary>
        /// <param name="clientFirstMessage">First client message.</param>
        /// <param name="serverFirstMessage">First server message.</param>
        /// <param name="password">User password.</param>
        /// <param name="hash"><see cref="Hash"/> to use when calculating proof.</param>
        /// <param name="token">
        /// Channel binding token. Required when the client first message declared
        /// <see cref="ChannelBindingStatus.Required"/>, and must be omitted otherwise.
        /// </param>
        /// <param name="minimumIterations">
        /// Smallest iteration count to accept from the server. Defaults to
        /// <see cref="MinimumIterations"/>; lower it only to interoperate with a legacy server
        /// that predates RFC 7677, and understand that doing so weakens the derivation.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown when the token does not match the channel binding status of
        /// <paramref name="clientFirstMessage"/>, or when the server first message does not
        /// carry the nonce or iteration count the protocol requires.
        /// </exception>
        public ClientFinalMessage(ClientFirstMessage clientFirstMessage, ServerFirstMessage serverFirstMessage, string password, Hash hash, byte[]? token = null, int minimumIterations = MinimumIterations)
        {
            ValidateChannelBinding(clientFirstMessage.Gs2Header.ChannelBindingStatus, token);
            ValidateNonce(clientFirstMessage.Nonce?.Value, serverFirstMessage.Nonce?.Value);
            ValidateIterations(serverFirstMessage.Iterations?.Value, minimumIterations);

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

        /// <summary>
        /// Rejects a token that contradicts the GS2 header the client already committed to.
        /// </summary>
        /// <remarks>
        /// RFC 5802 defines the channel attribute as base64(gs2-header + cbind-data), where the
        /// cbind-data is present only for a "p=" header. Both mismatches produce a message a
        /// conforming server rejects, so failing here turns a silent authentication failure into
        /// an error that names the cause.
        /// </remarks>
        /// <param name="status">Binding status declared in the client first message.</param>
        /// <param name="token">Channel binding token supplied by the caller.</param>
        /// <exception cref="ArgumentException">Thrown when the two disagree.</exception>
        private static void ValidateChannelBinding(ChannelBindingStatus status, byte[]? token)
        {
            var hasToken = token is { Length: > 0 };

            if (status == ChannelBindingStatus.Required && !hasToken)
            {
                throw new ArgumentException(
                    "The client first message requires channel binding, so a channel binding token is required. " +
                    "Without one the message advertises a binding it does not carry.",
                    nameof(token));
            }

            if (status != ChannelBindingStatus.Required && hasToken)
            {
                throw new ArgumentException(
                    $"The client first message declared {status}, so a channel binding token cannot be used. " +
                    $"Use {nameof(ChannelBindingStatus)}.{nameof(ChannelBindingStatus.Required)} to bind the exchange to the channel.",
                    nameof(token));
            }
        }

        /// <summary>
        /// Rejects a server nonce that does not extend the nonce the client chose.
        /// </summary>
        /// <remarks>
        /// RFC 5802 section 5.1 requires the client to verify that the initial part of the nonce
        /// in the server first message is the nonce it sent. Skipping the check hands the server
        /// sole control of the nonce, which is the client's half of the replay protection: an
        /// attacker replaying a captured server first message would otherwise be answered with a
        /// proof over a nonce the client never contributed to.
        /// </remarks>
        /// <param name="clientNonce">Nonce sent in the client first message.</param>
        /// <param name="serverNonce">Nonce returned in the server first message.</param>
        /// <exception cref="ArgumentException">Thrown when the server nonce does not extend the client nonce.</exception>
        private static void ValidateNonce(string? clientNonce, string? serverNonce)
        {
            if (string.IsNullOrEmpty(clientNonce))
            {
                throw new ArgumentException(
                    "The client first message has no nonce, so the server nonce cannot be verified against it.",
                    nameof(clientNonce));
            }

            if (serverNonce is null)
            {
                throw new ArgumentException(
                    "The server first message has no nonce.",
                    nameof(serverNonce));
            }

            if (!serverNonce.StartsWith(clientNonce, StringComparison.Ordinal))
            {
                throw new ArgumentException(
                    "The server nonce does not begin with the client nonce, so the server did not " +
                    "echo the nonce this exchange started with. Treat the exchange as compromised.",
                    nameof(serverNonce));
            }

            // A server that echoes the client nonce unchanged has contributed no randomness of its
            // own, leaving the exchange replayable from the server's side.
            if (serverNonce.Length == clientNonce.Length)
            {
                throw new ArgumentException(
                    "The server nonce is the client nonce with nothing appended, so the server " +
                    "contributed no randomness to the exchange.",
                    nameof(serverNonce));
            }
        }

        /// <summary>
        /// Rejects an iteration count outside the range this library will derive a key for.
        /// </summary>
        /// <param name="iterations">Iteration count from the server first message.</param>
        /// <param name="minimum">Smallest acceptable iteration count.</param>
        /// <exception cref="ArgumentException">Thrown when the count is missing or out of range.</exception>
        private static void ValidateIterations(int? iterations, int minimum)
        {
            if (iterations is not { } value)
            {
                throw new ArgumentException(
                    "The server first message has no iteration count.",
                    nameof(iterations));
            }

            if (value < minimum)
            {
                throw new ArgumentException(
                    $"The server asked for {value} iterations, below the minimum of {minimum}. " +
                    $"RFC 7677 requires at least {MinimumIterations}; a lower count weakens the " +
                    "derived key against an attacker who captures the proof.",
                    nameof(iterations));
            }

            if (value > MaximumIterations)
            {
                throw new ArgumentException(
                    $"The server asked for {value} iterations, above the maximum of {MaximumIterations}. " +
                    "Deriving the key would take long enough to stall the client.",
                    nameof(iterations));
            }
        }

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

            // The signature is raw MAC output, so it has to be base64 encoded to match
            // the "v=" value the server sends. Decoding it as UTF-8 would replace every
            // invalid byte sequence with U+FFFD and make the comparison impossible.
            ServerSignature = Convert.ToBase64String(hash.ComputeHash(auth, serverKey));

            Proof = new ClientProofAttribute(clientKey.ExclusiveOr(signature));
        }
    }
}
