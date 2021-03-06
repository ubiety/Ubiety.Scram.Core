using System;
using System.Text;
using Shouldly;
using Ubiety.Scram.Core;
using Xunit;

namespace Ubiety.Scram.Test
{
    public class HashTests
    {
        [Theory]
        [InlineData("pencil", "1d96ee3a529b5a5f9e47c01f229a2cb8a6e15f7d")]
        public void When_HashIsSha1_ExpectResultToEqualExpectedValue(string password, string expectedValue)
        {
            var salt = Convert.FromBase64String("QSXCR+Q6sek8bf92");
            const int i = 4096;

            var hash = Hash.Sha1();

            var pass = hash.ComputeHash(Encoding.UTF8.GetBytes(password), salt, i);

            pass.ShouldBe(HexToByte(expectedValue));
        }

        private static byte[] HexToByte(string value)
        {
            var numChars = value.Length;
            var bytes = new byte[numChars / 2];
            for (var i = 0; i < numChars; i += 2) bytes[i / 2] = Convert.ToByte(value.Substring(i, 2), 16);

            return bytes;
        }

        [Theory]
        [InlineData("pencil", "A97517AE572F9DAC71586D340DD460562A11DA09D4A6E5F9AFEDC4675ADD8556")]
        public void When_HashIsSha256_ExpectResultToEqualExpectedValue(string password, string expectedValue)
        {
            var salt = Convert.FromBase64String("QSXCR+Q6sek8bf92");
            const int i = 4096;

            var hash = Hash.Sha256();

            var pass = hash.ComputeHash(Encoding.UTF8.GetBytes(password), salt, i);

            pass.ShouldBe(HexToByte(expectedValue));
        }
    }
}
