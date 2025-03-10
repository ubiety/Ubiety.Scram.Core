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

namespace Ubiety.Scram.Core
{
    /// <summary>
    /// Provides extension methods for binary operations.
    /// </summary>
    public static class BinaryExtensions
    {
        /// <summary>
        /// Performs a bitwise exclusive OR (XOR) operation on two byte arrays of the same length.
        /// </summary>
        /// <param name="originalBytes">The first byte array to be XORed.</param>
        /// <param name="compareBytes">The second byte array to be XORed. Must be the same length as the first byte array.</param>
        /// <returns>A byte array containing the result of the XOR operation.</returns>
        /// <exception cref="ArgumentException">Thrown when the lengths of the two byte arrays do not match.</exception>
        public static byte[] ExclusiveOr(this byte[] originalBytes, byte[] compareBytes)
        {
            if (originalBytes.Length != compareBytes.Length)
            {
                throw new ArgumentException($"Argument {nameof(originalBytes)} is not the same length as argument {nameof(compareBytes)}");
            }

            var result = new byte[originalBytes.Length];
            for (var i = 0; i < originalBytes.Length; ++i)
            {
                result[i] = (byte)(originalBytes[i] ^ compareBytes[i]);
            }

            return result;
        }
    }
}
