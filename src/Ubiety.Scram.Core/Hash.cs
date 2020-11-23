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
using System.Linq;
using System.Security.Cryptography;

namespace Ubiety.Scram.Core
{
    /// <summary>
    ///     Hashing methods.
    /// </summary>
    public class Hash
    {
        private readonly HashAlgorithm _hashAlgorithm;
        private readonly Func<byte[], HMAC> _hmacFactory;

        private Hash(HashAlgorithm algorithm, Func<byte[], HMAC> hmacFactory)
        {
            _hashAlgorithm = algorithm;
            _hmacFactory = hmacFactory;
        }

        /// <summary>
        ///     Gets a new SHA1 based hash.
        /// </summary>
        /// <returns><see cref="Hash"/> set to use SHA1.</returns>
        public static Hash Sha1()
        {
            return new Hash(new SHA1Managed(), GetHmacSha1);
        }

        /// <summary>
        ///     Gets a new SHA256 hash.
        /// </summary>
        /// <returns><see cref="Hash"/> set to use SHA256.</returns>
        public static Hash Sha256()
        {
            return new Hash(new SHA256Managed(), GetHmacSha256);
        }

        /// <summary>
        ///     Gets a new SHA512 hash.
        /// </summary>
        /// <returns><see cref="Hash"/> set to use SHA512.</returns>
        public static Hash Sha512()
        {
            return new Hash(new SHA512Managed(), GetHmacSha512);
        }

        /// <summary>
        ///     Compute a hash.
        /// </summary>
        /// <param name="value">bytes to hash.</param>
        /// <returns>byte array of the hash output.</returns>
        public byte[] ComputeHash(byte[] value)
        {
            return _hashAlgorithm.ComputeHash(value);
        }

        /// <summary>
        ///     Compute a hash.
        /// </summary>
        /// <param name="value">bytes to hash.</param>
        /// <param name="key">Hash key.</param>
        /// <returns>byte array of the hash output.</returns>
        public byte[] ComputeHash(byte[] value, byte[] key)
        {
            var hmacAlgorithm = _hmacFactory(key);
            return hmacAlgorithm.ComputeHash(value);
        }

        /// <summary>
        ///     Compute a hash.
        /// </summary>
        /// <param name="password">byte array of a password.</param>
        /// <param name="salt">byte array of the hash salt.</param>
        /// <param name="iterations">number of times to iterate the hash.</param>
        /// <returns>byte array of the hash output.</returns>
        public byte[] ComputeHash(byte[] password, IEnumerable<byte> salt, int iterations)
        {
            var one = BitConverter.GetBytes(1);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(one);
            }

            var completeSalt = salt.Concat(one).ToArray();
            var iteration = ComputeHash(completeSalt, password);
            var final = iteration;

            for (var i = 1; i < iterations; ++i)
            {
                var temp = ComputeHash(iteration, password);
                final = final.ExclusiveOr(temp);
                iteration = temp;
            }

            return final;
        }

        private static HMAC GetHmacSha1(byte[] key)
        {
            return new HMACSHA1(key);
        }

        private static HMAC GetHmacSha256(byte[] key)
        {
            return new HMACSHA256(key);
        }

        private static HMAC GetHmacSha512(byte[] key)
        {
            return new HMACSHA512(key);
        }
    }
}
