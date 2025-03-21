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

namespace Ubiety.Scram.Core.Attributes
{
    /// <summary>
    /// Represents an SCRAM (Salted Challenge Response Authentication Mechanism) attribute
    /// for the authorization identity in a communication protocol.
    /// </summary>
    /// <remarks>
    /// This attribute is identified by the character name 'a' and is used to
    /// handle the authorization identity value as part of the authentication process.
    /// It extends the base functionality of the generic <see cref="ScramAttribute{TValue}"/> class
    /// with a specific implementation for handling string values for the authorization identity.
    /// </remarks>
    public class AuthorizationIdentityAttribute : ScramAttribute<string>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="AuthorizationIdentityAttribute"/> class.
        /// </summary>
        /// <param name="value">String version of the attribute value.</param>
        public AuthorizationIdentityAttribute(string value)
            : base(AuthorizationIdentityName, value)
        {
        }
    }
}
