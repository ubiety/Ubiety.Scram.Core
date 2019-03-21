using System;
using System.Text;
using Shouldly;
using Ubiety.Scram.Core.Messages;
using Xunit;

namespace Ubiety.Scram.Test.Messages
{
    public class ServerFirstMessageTests
    {
        [Theory]
        [InlineData("r=fyko+d2lbbFgONRv9qkxdawL3rfcNHYJY1ZVvWVs7j,s=QSXCR+Q6sek8bf92,i=4096")]
        public void When_ProvidedWithAMessage_ParseShouldSetTheProperties(string message)
        {
            var response = ServerFirstMessage.ParseResponse(message);

            response.Iterations.Value.ShouldBe(4096);
            response.Nonce.Value.ShouldBe("fyko+d2lbbFgONRv9qkxdawL3rfcNHYJY1ZVvWVs7j");
            response.Salt.ToString().ShouldBe("s = QSXCR+Q6sek8bf92");
        }

        [Theory]
        [InlineData("r=fyko+d2lbbFgONRv9qkxdawL3rfcNHYJY1ZVvWVs7j")]
        [InlineData("s=QSXCR+Q6sek8bf92")]
        [InlineData("i=4096")]
        [InlineData("r=fyko+d2lbbFgONRv9qkxdawL3rfcNHYJY1ZVvWVs7j,s=QSXCR+Q6sek8bf92")]
        [InlineData("s=QSXCR+Q6sek8bf92,i=4096")]
        [InlineData("r=fyko+d2lbbFgONRv9qkxdawL3rfcNHYJY1ZVvWVs7j,i=4096")]
        public void When_ProvidedWithInvalidData_ParseShouldThrowAnException(string response)
        {
            Should.Throw<InvalidOperationException>(() =>
            {
                var _ = ServerFirstMessage.ParseResponse(response);
            });
        }

        [Theory]
        [InlineData("e=message")]
        [InlineData("r=data,e=message")]
        public void When_ProvidedWithAnError_ParseShouldThrowAnException(string response)
        {
            Should.Throw<InvalidOperationException>(() =>
            {
                var _ = ServerFirstMessage.ParseResponse(response);
            });
        }

        [Fact]
        public void When_CreatedWithByteConstructor_ThePropertiesShouldBeValid()
        {
            var message = new ServerFirstMessage(4096, "nonce", Convert.FromBase64String("salt"));

            message.Iterations.Value.ShouldBe(4096);
            message.Nonce.Value.ShouldBe("nonce");
            message.Salt.ToString().ShouldBe("s = salt");
        }

        [Fact]
        public void When_CreatedWithStringConstructor_ThePropertiesShouldBeValid()
        {
            var message = new ServerFirstMessage(4096, "nonce", "salt");

            message.Iterations.Value.ShouldBe(4096);
            message.Nonce.Value.ShouldBe("nonce");
            message.Salt.ToString().ShouldBe("s = salt");
        }

    }
}
