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
using System.Globalization;

namespace Ubiety.Scram.Core.Attributes
{
    /// <summary>
    /// Represents the SCRAM attribute for iteration counts used in SCRAM-based authentication.
    /// </summary>
    public class IterationsAttribute : ScramAttribute<int>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="IterationsAttribute"/> class.
        /// </summary>
        /// <param name="value">Integer value of the iterations.</param>
        public IterationsAttribute(int value)
            : base(IterationsName, value)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="IterationsAttribute"/> class.
        /// </summary>
        /// <param name="value">String value of the iterations.</param>
        /// <exception cref="FormatException">Thrown when the value is not an integer this library can represent.</exception>
        public IterationsAttribute(string value)
            : base(IterationsName, ParseCount(value))
        {
        }

        /// <summary>
        /// Converts the wire value to an integer.
        /// </summary>
        /// <remarks>
        /// A count too large for <see cref="int"/> has to fail as a malformed attribute rather
        /// than an <see cref="OverflowException"/>: the value comes from the peer, and the
        /// TryParse methods that read these messages only treat a
        /// <see cref="FormatException"/> as a parse failure.
        /// </remarks>
        /// <param name="value">String value of the iterations.</param>
        /// <returns>The iteration count.</returns>
        /// <exception cref="FormatException">Thrown when the value is not an integer this library can represent.</exception>
        private static int ParseCount(string value)
        {
            if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var iterations))
            {
                throw new FormatException($"'{value}' is not a valid iteration count.");
            }

            return iterations;
        }
    }
}
