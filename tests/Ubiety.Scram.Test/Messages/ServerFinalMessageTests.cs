﻿using System;
using Shouldly;
using Xunit;
using Ubiety.Scram.Core.Messages;

namespace Ubiety.Scram.Test.Messages
{
    public class ServerFinalMessageTests
    {
        [Fact]
        public void When_ParsingAMessage_PropertiesShouldBeValid()
        {
            var message = ServerFinalMessage.ParseResponse("v=rmF9pqV8S7suAoZWja4dJRkFsKQ=");

            message.ServerSignature.Value.ShouldBe(HexToByte("ae617da6a57c4bbb2e0286568dae1d251905b0a4"));
        }

        [Fact]
        public void When_MessageAnError_ParseShouldThrowAnException()
        {
            Should.Throw<InvalidOperationException>(() =>
            {
                var _ = ServerFinalMessage.ParseResponse("e=error");
            });
        }

        [Fact]
        public void When_MessageDoesNotContainASignature_ParseShouldThrowAnException()
        {
            Should.Throw<InvalidOperationException>(() =>
            {
                var _ = ServerFinalMessage.ParseResponse("r=invalid");
            });
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