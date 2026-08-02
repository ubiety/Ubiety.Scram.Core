using Shouldly;
using Ubiety.Scram.Core.Messages;
using Xunit;

namespace Ubiety.Scram.Test.Messages
{
    public class ClientFirstMessageTests
    {
        [Fact]
        public void When_NonceIsSet_ExpectNoncePropertyToMatch()
        {
            var message = new ClientFirstMessage("", "fyko+d2lbbFgONRv9qkxdawL");

            message.Nonce?.ToString().ShouldBe("r=fyko+d2lbbFgONRv9qkxdawL");
        }

        [Fact]
        public void When_UsernameAndNonceAreSet_ExpectBareMessageToMatch()
        {
            var message = new ClientFirstMessage("user", "fyko+d2lbbFgONRv9qkxdawL");

            message.BareMessage.ShouldBe("n=user,r=fyko+d2lbbFgONRv9qkxdawL");
        }

        [Fact]
        public void When_UsernameAndNonceAreSet_ExpectMessageToMatch()
        {
            var message = new ClientFirstMessage("user", "fyko+d2lbbFgONRv9qkxdawL");

            message.Message.ShouldBe("n,,n=user,r=fyko+d2lbbFgONRv9qkxdawL");
        }

        [Fact]
        public void When_UsernameIsSet_ExpectUsernamePropertyToMatch()
        {
            var message = new ClientFirstMessage("user", "");

            message.Username?.ToString().ShouldBe("n=user");
        }

        [Theory]
        [InlineData("n,,n=I\u00ADX,r=nonce")]
        [InlineData("n,,n=user\u0007,r=nonce")]
        [InlineData("n,,n=\u0627\u0031,r=nonce")]
        public void When_TheUsernameIsNotPrepped_ItShouldBeAParseFailure(string message)
        {
            ClientFirstMessage.TryParse(message, out _).ShouldBeFalse();
        }

        [Theory]
        [InlineData("p=tls-future,,n=user,r=nonce")]
        [InlineData("p=,,n=user,r=nonce")]
        public void When_TheBindingTypeIsUnknown_ItShouldBeAParseFailure(string message)
        {
            // Reading it as tls-unique would let the exchange continue over a binding type neither
            // peer agreed on, and rewrite the header the proof was signed over.
            ClientFirstMessage.TryParse(message, out _).ShouldBeFalse();
        }

        [Theory]
        [InlineData("n,,")]
        [InlineData("y,,")]
        [InlineData("p=tls-exporter,,")]
        [InlineData("p=tls-unique,,")]
        [InlineData("p=tls-server-end-point,,")]
        public void When_TheGs2HeaderIsValid_TheMessageShouldRoundTrip(string header)
        {
            var wire = $"{header}n=user,r=nonce";

            ClientFirstMessage.TryParse(wire, out var message).ShouldBeTrue();

            message.Message.ShouldBe(wire);
        }

        [Fact]
        public void When_TheUsernameIsPrepped_TheMessageShouldRoundTrip()
        {
            const string wire = "n,,n=IX,r=nonce";

            ClientFirstMessage.TryParse(wire, out var message).ShouldBeTrue();

            // The proof is computed over these bytes, so parsing must reproduce them exactly.
            message.Message.ShouldBe(wire);
        }
    }
}
