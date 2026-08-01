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
    /// Represents a utility class for cryptographic hashing operations using SHA-1, SHA-256, or SHA-512.
    /// Provides methods to compute hashes for byte arrays with optional key-based hashing or iterative hashing for password-based systems.
    /// </summary>
    public class Hash
    {
        private readonly HashAlgorithmName _algorithm;
        private readonly int _hashLength;

        private Hash(HashAlgorithmName algorithm, int hashLength)
        {
            _algorithm = algorithm;
            _hashLength = hashLength;
        }

        /// <summary>
        /// Gets a new SHA1 hash.
        /// </summary>
        /// <returns><see cref="Hash"/> set to use SHA1.</returns>
        public static Hash Sha1()
        {
            return new Hash(HashAlgorithmName.SHA1, SHA1.HashSizeInBytes);
        }

        /// <summary>
        /// Gets a new SHA256 hash.
        /// </summary>
        /// <returns><see cref="Hash"/> set to use SHA256.</returns>
        public static Hash Sha256()
        {
            return new Hash(HashAlgorithmName.SHA256, SHA256.HashSizeInBytes);
        }

        /// <summary>
        /// Gets a new SHA512 hash.
        /// </summary>
        /// <returns><see cref="Hash"/> set to use SHA512.</returns>
        public static Hash Sha512()
        {
            return new Hash(HashAlgorithmName.SHA512, SHA512.HashSizeInBytes);
        }

        /// <summary>
        /// Computes the hash of the provided byte array using the configured hash algorithm.
        /// </summary>
        /// <param name="value">The byte array to compute the hash for.</param>
        /// <returns>A byte array representing the computed hash.</returns>
        public byte[] ComputeHash(byte[] value)
        {
            return CryptographicOperations.HashData(_algorithm, value);
        }

        /// <summary>
        /// Computes the hash of the specified data using the given key.
        /// </summary>
        /// <param name="value">The data to be hashed.</param>
        /// <param name="key">The key used for hashing.</param>
        /// <returns>A byte array containing the computed hash.</returns>
        public byte[] ComputeHash(byte[] value, byte[] key)
        {
            return CryptographicOperations.HmacData(_algorithm, key, value);
        }

        /// <summary>
        /// Computes a cryptographic hash using the provided password, salt, and iterations.
        /// </summary>
        /// <param name="password">The byte array representing the password to hash.</param>
        /// <param name="salt">The byte array used as the salt for the hash.</param>
        /// <param name="iterations">The number of iterations to perform on the hash.</param>
        /// <returns>A byte array containing the computed hash value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="iterations"/> is less than one.</exception>
        public byte[] ComputeHash(byte[] password, IEnumerable<byte> salt, int iterations)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(iterations, 1);

            // The SCRAM Hi() function is PBKDF2 with an output length of one hash block.
            return Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt as byte[] ?? salt.ToArray(),
                iterations,
                _algorithm,
                _hashLength);
        }
    }
}
