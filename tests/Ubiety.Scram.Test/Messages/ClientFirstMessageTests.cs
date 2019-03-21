using Shouldly;
using Ubiety.Scram.Core.Messages;
using Xunit;

namespace Ubiety.Scram.Test.Messages
{
    public class ClientFirstMessageTests
    {
        [Fact]
        public void When_UsernameIsSet_ExpectUsernamePropertyToMatch()
        {
            var message = new ClientFirstMessage("user", "");

            message.Username.ToString().ShouldBe("n=user");
        }

        [Fact]
        public void When_NonceIsSet_ExpectNoncePropertyToMatch()
        {
            var message = new ClientFirstMessage("", "Nonce");

            message.Nonce.ToString().ShouldBe("r=Nonce");
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
    }
}
