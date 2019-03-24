﻿// This is free and unencumbered software released into the public domain.
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

        /// <summary>
        ///     Gets the GS2 header for the message.
        /// </summary>
        public string Gs2Header { get; } = "n,,";

        /// <summary>
        ///     Gets the username of the message.
        /// </summary>
        public UserAttribute Username { get; }

        /// <summary>
        ///     Gets the nonce of the message.
        /// </summary>
        public NonceAttribute Nonce { get; }

        /// <summary>
        ///     Gets the bare client message.
        /// </summary>
        public string BareMessage => $"{Username},{Nonce}";

        /// <summary>
        ///     Gets the client message with the GS2 header.
        /// </summary>
        public string Message => $"{Gs2Header}{BareMessage}";
    }
}