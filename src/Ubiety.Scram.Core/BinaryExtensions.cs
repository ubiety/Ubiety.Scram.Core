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

namespace Ubiety.Scram.Core
{
    /// <summary>
    ///     Extension methods for byte arrays.
    /// </summary>
    public static class BinaryExtensions
    {
        /// <summary>
        ///     Calculate the exclusive or result of two byte arrays.
        /// </summary>
        /// <param name="originalBytes">Original byte array.</param>
        /// <param name="compareBytes">Comparison byte array.</param>
        /// <returns>Byte array of the result.</returns>
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
