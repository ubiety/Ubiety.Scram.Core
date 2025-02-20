using System;
using Shouldly;
using Ubiety.Scram.Core.Exceptions;
using Ubiety.Scram.Core.Messages;
using Xunit;

namespace Ubiety.Scram.Test.Messages
{
    public class ServerFinalMessageTests
    {
        private static byte[] HexToByte(string value)
        {
            var numChars = value.Length;
            var bytes = new byte[numChars / 2];
            for (var i = 0; i < numChars; i += 2) bytes[i / 2] = Convert.ToByte(value.Substring(i, 2), 16);

            return bytes;
        }

        [Fact]
        public void When_MessageAnError_ParseShouldThrowAnException()
        {
            Should.Throw<MessageParseException>(() =>
            {
                var _ = ServerFinalMessage.Parse("e=error");
            });
        }

        [Fact]
        public void When_MessageDoesNotContainASignature_ParseShouldThrowAnException()
        {
            Should.Throw<MessageParseException>(() =>
            {
                var _ = ServerFinalMessage.Parse("r=invalid");
            });
        }

        [Fact]
        public void When_ParsingAMessage_PropertiesShouldBeValid()
        {
            var message = ServerFinalMessage.Parse("v=rmF9pqV8S7suAoZWja4dJRkFsKQ=");

            message.ServerSignature?.Value.ShouldBe(HexToByte("ae617da6a57c4bbb2e0286568dae1d251905b0a4"));
        }
    }
}
