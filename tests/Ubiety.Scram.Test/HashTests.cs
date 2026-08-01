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

        [Theory]
        [InlineData(
            "pencil",
            "97382788B15CBE09512D2D20B7E0B8832F8DBAB4B7388395440535CD9395E0FF" +
            "AA1625453B6FDE746412BBF903D4BC1D5F448D57F2AC3DD1D2C04979A914EE65")]
        public void When_HashIsSha512_ExpectResultToEqualExpectedValue(string password, string expectedValue)
        {
            var salt = Convert.FromBase64String("QSXCR+Q6sek8bf92");
            const int i = 4096;

            var hash = Hash.Sha512();

            var pass = hash.ComputeHash(Encoding.UTF8.GetBytes(password), salt, i);

            pass.ShouldBe(HexToByte(expectedValue));
        }

        [Fact]
        public void When_HashingAKey_TheDigestShouldBeTheAlgorithmLength()
        {
            var value = Encoding.UTF8.GetBytes("Client Key");

            Hash.Sha1().ComputeHash(value).Length.ShouldBe(20);
            Hash.Sha256().ComputeHash(value).Length.ShouldBe(32);
            Hash.Sha512().ComputeHash(value).Length.ShouldBe(64);
        }

        [Fact]
        public void When_ComputingAnHmac_TheDigestShouldBeTheAlgorithmLength()
        {
            var value = Encoding.UTF8.GetBytes("Client Key");
            var key = Encoding.UTF8.GetBytes("salted password");

            Hash.Sha1().ComputeHash(value, key).Length.ShouldBe(20);
            Hash.Sha256().ComputeHash(value, key).Length.ShouldBe(32);
            Hash.Sha512().ComputeHash(value, key).Length.ShouldBe(64);
        }
    }
}
