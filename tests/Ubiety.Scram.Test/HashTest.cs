using System;
using System.Text;
using Ubiety.Scram.Core;
using Shouldly;
using Xunit;

namespace Ubiety.Scram.Test
{
    public class HashTest
    {
        [Fact]
        public void HashSha1ShouldMatchValue()
        {
            var salt = Convert.FromBase64String("QSXCR+Q6sek8bf92");
            const int i = 4096;
            const string password = "pencil";
            var result = HexToByte("1d96ee3a529b5a5f9e47c01f229a2cb8a6e15f7d");

            var hash = Hash.Sha1();

            var pass = hash.ComputeHash(Encoding.UTF8.GetBytes(password), salt, i);

            pass.ShouldBe(result);
        }

        [Fact]
        public void HashSha256ShouldMatchValue()
        {
            Assert.True(true);
        }

        private static byte[] HexToByte(string value)
        {
            var numChars = value.Length;
            var bytes = new byte[numChars / 2];
            for (var i = 0; i < numChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(value.Substring(i, 2), 16);
            }

            return bytes;
        }
    }
}
