using Shouldly;
using Ubiety.Scram.Core;
using Ubiety.Scram.Core.Messages;
using Xunit;

namespace Ubiety.Scram.Test.Messages
{
    public class ClientFinalMessageTests
    {
        [Fact]
        public void When_Created_PropertiesShouldBeValid()
        {
            var clientFirst = new ClientFirstMessage("user", "nonce");
            var serverFirst = new ServerFirstMessage(4096, "nonce", "salt", "");

            var message = new ClientFinalMessage(clientFirst, serverFirst, "", Hash.Sha1());

            message.Channel.ToString().ShouldBe("c=biws");
            message.Nonce.Value.ShouldBe("nonce");
            message.Proof?.ToString().ShouldBe("p=V1Skx762sbV1/HBaZx24cV2do3g=");
            message.Message.ShouldBe("c=biws,r=nonce,p=V1Skx762sbV1/HBaZx24cV2do3g=");
            message.MessageWithoutProof.ShouldBe("c=biws,r=nonce");
        }

        [Fact]
        public void When_ProofIsSetAsByteArray_PropertiesShouldBeValid()
        {
            var clientFirst = new ClientFirstMessage("user", "nonce");
            var serverFirst = new ServerFirstMessage(4096, "nonce", "salt", "");

            var message = new ClientFinalMessage(clientFirst, serverFirst, "", Hash.Sha1());
            // message.SetProof(Convert.FromBase64String("bf45fcbf7073d93d022466c94321745fe1c8e13b"));

            message.Channel.ToString().ShouldBe("c=biws");
            message.Nonce.Value.ShouldBe("nonce");
            message.Proof?.ToString().ShouldBe("p=V1Skx762sbV1/HBaZx24cV2do3g=");
            message.Message.ShouldBe("c=biws,r=nonce,p=V1Skx762sbV1/HBaZx24cV2do3g=");
            message.MessageWithoutProof.ShouldBe("c=biws,r=nonce");
        }

        [Fact]
        public void When_ProofIsSetAsString_PropertiesShouldBeValid()
        {
            var clientFirst = new ClientFirstMessage("user", "nonce");
            var serverFirst = new ServerFirstMessage(4096, "nonce", "salt", "");

            var message = new ClientFinalMessage(clientFirst, serverFirst, "", Hash.Sha1());
            // message.SetProof("bf45fcbf7073d93d022466c94321745fe1c8e13b");

            message.Channel.ToString().ShouldBe("c=biws");
            message.Nonce.Value.ShouldBe("nonce");
            message.Proof?.ToString().ShouldBe("p=V1Skx762sbV1/HBaZx24cV2do3g=");
            message.Message.ShouldBe("c=biws,r=nonce,p=V1Skx762sbV1/HBaZx24cV2do3g=");
            message.MessageWithoutProof.ShouldBe("c=biws,r=nonce");
        }
    }
}
