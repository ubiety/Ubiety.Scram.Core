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

using System;

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
        public UserAttribute(string value, bool fromWire = false)
            : base(UserName, Replace(value, fromWire))
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

        private static string Replace(string value, bool doReplace)
        {
            if (!doReplace)
            {
                return value;
            }

            var lastIndex = -1;
            while ((lastIndex = value.IndexOf('=', lastIndex + 1)) > -1)
            {
                var escapeCheck = value.Substring(lastIndex, 3);
                value = escapeCheck switch
                {
                    EqualReplacement => Replace(value, lastIndex, '=', EqualReplacement.Length),
                    CommaReplacement => Replace(value, lastIndex, ',', CommaReplacement.Length),
                    _ => throw new FormatException()
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
