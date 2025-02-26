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
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Ubiety.Scram.Core.Attributes
{
    /// <summary>
    /// Represents a SCRAM attribute used in the SCRAM authentication mechanism.
    /// </summary>
    public class ScramAttribute
    {
        /// <summary>
        /// Represents the attribute name for the authorization identity in the SCRAM authentication mechanism.
        /// </summary>
        protected const char AuthorizationIdentityName = 'a';

        /// <summary>
        /// Represents the attribute name for the username in the SCRAM authentication mechanism.
        /// </summary>
        protected const char UserName = 'n';

        /// <summary>
        /// Represents the attribute name for the message in the SCRAM authentication mechanism.
        /// </summary>
        protected const char MessageName = 'm';

        /// <summary>
        /// Represents the attribute name for the nonce used in the SCRAM authentication mechanism.
        /// </summary>
        protected const char NonceName = 'r';

        /// <summary>
        /// Represents the attribute name for the channel binding in the SCRAM authentication mechanism.
        /// </summary>
        protected const char ChannelName = 'c';

        /// <summary>
        /// Represents the attribute name for the salt value used in the SCRAM authentication mechanism.
        /// </summary>
        protected const char SaltName = 's';

        /// <summary>
        /// Represents the attribute name for the iteration count in the SCRAM authentication mechanism.
        /// </summary>
        protected const char IterationsName = 'i';

        /// <summary>
        /// Represents the attribute name for the client proof in the SCRAM authentication mechanism.
        /// </summary>
        protected const char ClientProofName = 'p';

        /// <summary>
        /// Represents the attribute name for the server's signature in the SCRAM authentication mechanism.
        /// </summary>
        protected const char ServerSignatureName = 'v';

        /// <summary>
        /// Represents the attribute name used to indicate errors within the SCRAM authentication mechanism.
        /// </summary>
        protected const char ErrorName = 'e';

        // language=regex
        private const string ScramRegex = @"(?:(?<gs2>^[ny]?(?:p=.+?)?),)?(?:,?(?<attr>.=[a-zA-Z0-9\+\=]+?),?)+$";

        /// <summary>
        ///     Initializes a new instance of the <see cref="ScramAttribute"/> class.
        /// </summary>
        /// <param name="name">Attribute name.</param>
        protected ScramAttribute(char name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets the name of the SCRAM attribute.
        /// </summary>
        protected char Name { get; }

        /// <summary>
        /// Parses a string of concatenated attributes and returns a collection of parsed attribute instances.
        /// </summary>
        /// <param name="attributes">The string containing concatenated attribute values to parse.</param>
        /// <returns>A collection of <see cref="ScramAttribute"/> objects representing the parsed attributes.</returns>
        public static ICollection<ScramAttribute> ParseAll(string attributes)
        {
            var regex = new Regex(ScramRegex, RegexOptions.Compiled | RegexOptions.CultureInvariant);

            var match = regex.Match(attributes);

            if (!match.Success)
            {
                throw new FormatException();
            }

            var attrs = new List<ScramAttribute>();

            if (match.Groups["gs2"].Success)
            {
                var gs2 = new Gs2Attribute(match.Groups["gs2"].Value);
                attrs.Add(gs2);
            }

            foreach (Capture attribute in match.Groups["attr"].Captures)
            {
                attrs.Add(Parse(attribute.Value));
            }

            return attrs;
        }

        /// <summary>
        /// Parses the provided attribute string into a specific type of <see cref="ScramAttribute"/>.
        /// </summary>
        /// <param name="attribute">A string representation of the attribute in the format "key=value".</param>
        /// <returns>An instance of the appropriate <see cref="ScramAttribute"/> type based on the key.</returns>
        /// <exception cref="FormatException">
        /// Thrown if the attribute string does not contain exactly one '=' or if the key portion of the string
        /// is not a single character.
        /// </exception>
        public static ScramAttribute Parse(string attribute)
        {
            var parts = attribute.Split(['='], 2);
            if (parts.Length != 2)
            {
                throw new FormatException();
            }

            if (parts[0].Length > 1)
            {
                throw new FormatException();
            }

            return parts[0][0] switch
            {
                AuthorizationIdentityName => new AuthorizationIdentityAttribute(parts[1]),
                UserName => new UserAttribute(parts[1], true),
                NonceName => new NonceAttribute(parts[1]),
                ChannelName => new ChannelAttribute(parts[1]),
                SaltName => new SaltAttribute(parts[1]),
                IterationsName => new IterationsAttribute(parts[1]),
                ClientProofName => new ClientProofAttribute(parts[1]),
                ServerSignatureName => new ServerSignatureAttribute(parts[1]),
                ErrorName => new ErrorAttribute(parts[1]),
                _ => new UnknownAttribute(parts[0][0], parts[1])
            };
        }
    }
}
