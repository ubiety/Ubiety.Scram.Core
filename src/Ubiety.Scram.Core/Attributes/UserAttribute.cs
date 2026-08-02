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
using Ubiety.Stringprep.Core.Exceptions;

namespace Ubiety.Scram.Core.Attributes
{
    /// <summary>
    /// Represents a SCRAM user attribute with specific formatting and escaping rules.
    /// </summary>
    public class UserAttribute : ScramAttribute<string>
    {
        private const string EqualReplacement = "=3D";
        private const string CommaReplacement = "=2C";

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserAttribute"/> class.
        /// </summary>
        /// <param name="value">String value of the username.</param>
        /// <param name="fromWire">Indicates if the value is escaped from the server.</param>
        /// <exception cref="FormatException">
        /// Thrown when <paramref name="fromWire"/> is <c>true</c> and the value is not a valid
        /// escaped, SASLprep'd username.
        /// </exception>
        /// <exception cref="ProhibitedValueException">
        /// Thrown when <paramref name="fromWire"/> is <c>false</c> and the username contains a
        /// character SASLprep prohibits.
        /// </exception>
        /// <exception cref="BidirectionalFormatException">
        /// Thrown when <paramref name="fromWire"/> is <c>false</c> and the username mixes text
        /// directions in a way SASLprep forbids.
        /// </exception>
        public UserAttribute(string value, bool fromWire = false)
            : base(UserName, Prepare(value, fromWire))
        {
        }

        /// <summary>
        /// Converts the current instance of <see cref="UserAttribute"/> to its string representation.
        /// </summary>
        /// <returns>
        /// A string representation of the <see cref="UserAttribute"/>, where the value is encoded by replacing
        /// specific characters ("=" and ",") with their respective replacements.
        /// </returns>
        public override string ToString()
        {
            var printableValue = Value.Replace("=", EqualReplacement).Replace(",", CommaReplacement);
            return $"{Name}={printableValue}";
        }

        /// <summary>
        /// Unescapes the wire form of a username and applies SASLprep to it.
        /// </summary>
        /// <remarks>
        /// RFC 5802 section 5.1 requires the username to be SASLprep'd, so that two Unicode
        /// spellings of the same name cannot authenticate as two different principals. Preparing
        /// a value on its way out is a transformation; a value arriving from the wire was already
        /// prepared by the peer, so it is only verified. Rewriting an inbound username would
        /// change the bytes the peer signed its proof over and break verification.
        /// </remarks>
        /// <param name="value">Username, escaped when it comes from the wire.</param>
        /// <param name="fromWire">Whether the value arrived from the wire.</param>
        /// <returns>The prepared username.</returns>
        private static string Prepare(string value, bool fromWire)
        {
            var unescaped = Replace(value, fromWire);

            return fromWire ? VerifyPrepared(unescaped) : SaslPrep.Run(unescaped);
        }

        /// <summary>
        /// Rejects an inbound username that is not already SASLprep'd.
        /// </summary>
        /// <remarks>
        /// The stringprep failures become a <see cref="FormatException"/> because that is what the
        /// message TryParse methods treat as a parse failure; letting the original types through
        /// would turn a malformed message from a peer into an exception escaping TryParse.
        /// </remarks>
        /// <param name="value">Unescaped username as it arrived.</param>
        /// <returns>The value unchanged.</returns>
        /// <exception cref="FormatException">Thrown when the value is not a valid SASLprep'd username.</exception>
        private static string VerifyPrepared(string value)
        {
            string prepared;

            try
            {
                prepared = SaslPrep.Run(value);
            }
            catch (ProhibitedValueException exception)
            {
                throw new FormatException("The username contains a character SASLprep prohibits.", exception);
            }
            catch (BidirectionalFormatException exception)
            {
                throw new FormatException("The username mixes text directions in a way SASLprep forbids.", exception);
            }

            if (!string.Equals(prepared, value, StringComparison.Ordinal))
            {
                throw new FormatException(
                    "The username is not SASLprep'd, so it does not match the form RFC 5802 requires " +
                    "on the wire. Accepting it would let a second spelling of a name authenticate as " +
                    "that name.");
            }

            return value;
        }

        private static string Replace(string value, bool doReplace)
        {
            if (!doReplace)
            {
                return value;
            }

            var lastIndex = -1;
            while ((lastIndex = value.IndexOf('=', lastIndex + 1)) > -1)
            {
                // An '=' too close to the end cannot start a complete escape sequence.
                if (lastIndex + 3 > value.Length)
                {
                    throw new FormatException($"Username contains a truncated escape sequence at index {lastIndex}.");
                }

                var escapeCheck = value.Substring(lastIndex, 3);
                value = escapeCheck switch
                {
                    EqualReplacement => Replace(value, lastIndex, '=', EqualReplacement.Length),
                    CommaReplacement => Replace(value, lastIndex, ',', CommaReplacement.Length),
                    _ => throw new FormatException($"Username contains an invalid escape sequence '{escapeCheck}'."),
                };
            }

            return value;
        }

        private static string Replace(string value, int index, char replacement, int len)
        {
            var temp1 = value[..index];
            var temp2 = value[(index + len)..];
            return $"{temp1}{replacement}{temp2}";
        }
    }
}
