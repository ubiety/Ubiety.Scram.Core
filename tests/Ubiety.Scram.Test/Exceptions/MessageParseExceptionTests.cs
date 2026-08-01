using System;
using Shouldly;
using Ubiety.Scram.Core.Exceptions;
using Xunit;

namespace Ubiety.Scram.Test.Exceptions
{
    public class MessageParseExceptionTests
    {
        [Fact]
        public void When_ConstructedWithoutAMessage_ShouldStillCarryADescription()
        {
            var exception = new MessageParseException();

            exception.Message.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public void When_ConstructedWithAMessage_ShouldCarryIt()
        {
            var exception = new MessageParseException("the nonce was missing");

            exception.Message.ShouldBe("the nonce was missing");
        }

        [Fact]
        public void When_ConstructedWithAnInnerException_ShouldCarryBoth()
        {
            var inner = new FormatException("bad base64");

            var exception = new MessageParseException("the salt was unreadable", inner);

            exception.Message.ShouldBe("the salt was unreadable");
            exception.InnerException.ShouldBeSameAs(inner);
        }
    }
}
