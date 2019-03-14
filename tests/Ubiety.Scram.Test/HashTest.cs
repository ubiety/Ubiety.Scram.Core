using System;
using System.Text;
using FluentAssertions;
using Ubiety.Scram.Core;
using Xunit;

namespace Ubiety.Scram.Test
{
    public class HashTest
    {
        [Fact]
        public void HashShouldMatchValue()
        {
            var salt = Convert.FromBase64String("QSXCR+Q6sek8bf92");
            var i = 4096;
            var password = "pencil";
            var result = HexToByte("1d96ee3a529b5a5f9e47c01f229a2cb8a6e15f7d");

            var hash = Hash.Sha1();

            var pass = hash.ComputeHash(Encoding.UTF8.GetBytes(password), salt, i);

            pass.Should().Equal(result);
        }

        private byte[] HexToByte(string value)
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
