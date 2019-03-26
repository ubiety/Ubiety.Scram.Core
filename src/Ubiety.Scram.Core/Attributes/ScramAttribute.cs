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
    ///     Base scram attribute.
    /// </summary>
    public class ScramAttribute
    {
        /// <summary>
        ///     Authorization identity attribute name.
        /// </summary>
        protected const char AuthorizationIdentityName = 'a';

        /// <summary>
        ///     User attribute name.
        /// </summary>
        protected const char UserName = 'n';

        /// <summary>
        ///     Message attribute name.
        /// </summary>
        protected const char MessageName = 'm';

        /// <summary>
        ///     Nonce attribute name.
        /// </summary>
        protected const char NonceName = 'r';

        /// <summary>
        ///     Channel attribute name.
        /// </summary>
        protected const char ChannelName = 'c';

        /// <summary>
        ///     Salt attribute name.
        /// </summary>
        protected const char SaltName = 's';

        /// <summary>
        ///     Iterations attribute name.
        /// </summary>
        protected const char IterationsName = 'i';

        /// <summary>
        ///     Client proof attribute name.
        /// </summary>
        protected const char ClientProofName = 'p';

        /// <summary>
        ///     Server signature attribute name.
        /// </summary>
        protected const char ServerSignatureName = 'v';

        /// <summary>
        ///     Error attribute name.
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
        ///     Gets the attribute name.
        /// </summary>
        protected char Name { get; }

        /// <summary>
        ///     Parse all the attributes.
        /// </summary>
        /// <param name="attributes">List of attribute strings to parse.</param>
        /// <returns>List of attribute classes.</returns>
        public static ICollection<ScramAttribute> ParseAll(string attributes)
        {
            var regex = new Regex(ScramRegex, RegexOptions.Compiled | RegexOptions.CultureInvariant);

            var match = regex.Match(attributes);

            if (!match.Success)
            {
                throw new FormatException();
            }

            var attrs = new List<ScramAttribute>();

            foreach (Capture attribute in match.Groups["attr"].Captures)
            {
                attrs.Add(Parse(attribute.Value));
            }

            return attrs;
        }

        /// <summary>
        ///     Parse an attribute.
        /// </summary>
        /// <param name="attribute">String value of the attribute.</param>
        /// <returns>Attribute class.</returns>
        public static ScramAttribute Parse(string attribute)
        {
            var parts = attribute.Split(new[] { '=' }, 2);
            if (parts.Length != 2)
            {
                throw new FormatException();
            }

            if (parts[0].Length > 1)
            {
                throw new FormatException();
            }

            switch (parts[0][0])
            {
                case AuthorizationIdentityName: return new AuthorizationIdentityAttribute(parts[1]);
                case UserName: return new UserAttribute(parts[1], true);
                case NonceName: return new NonceAttribute(parts[1]);
                case ChannelName: return new ChannelAttribute(parts[1]);
                case SaltName: return new SaltAttribute(parts[1]);
                case IterationsName: return new IterationsAttribute(parts[1]);
                case ClientProofName: return new ClientProofAttribute(parts[1]);
                case ServerSignatureName: return new ServerSignatureAttribute(parts[1]);
                case ErrorName: return new ErrorAttribute(parts[1]);
                default: return new UnknownAttribute(parts[0][0], parts[1]);
            }
        }
    }
}
