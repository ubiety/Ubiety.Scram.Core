using System;
using Shouldly;
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
            var serverFirst = new ServerFirstMessage(4096, "nonce", "salt");

            var message = new ClientFinalMessage(clientFirst, serverFirst);

            message.Channel.Header.ShouldBe("biws");
            message.Nonce.Value.ShouldBe("nonce");
            message.Proof.ShouldBeNull();
            message.Message.ShouldBe("c=biws,r=nonce,");
            message.MessageWithoutProof.ShouldBe("c=biws,r=nonce");
        }

        [Fact]
        public void When_ProofIsSetAsByteArray_PropertiesShouldBeValid()
        {
            var clientFirst = new ClientFirstMessage("user", "nonce");
            var serverFirst = new ServerFirstMessage(4096, "nonce", "salt");

            var message = new ClientFinalMessage(clientFirst, serverFirst);
            message.SetProof(Convert.FromBase64String("bf45fcbf7073d93d022466c94321745fe1c8e13b"));

            message.Channel.Header.ShouldBe("biws");
            message.Nonce.Value.ShouldBe("nonce");
            message.Proof?.ToString().ShouldBe("p=bf45fcbf7073d93d022466c94321745fe1c8e13b");
            message.Message.ShouldBe("c=biws,r=nonce,p=bf45fcbf7073d93d022466c94321745fe1c8e13b");
            message.MessageWithoutProof.ShouldBe("c=biws,r=nonce");
        }

        [Fact]
        public void When_ProofIsSetAsString_PropertiesShouldBeValid()
        {
            var clientFirst = new ClientFirstMessage("user", "nonce");
            var serverFirst = new ServerFirstMessage(4096, "nonce", "salt");

            var message = new ClientFinalMessage(clientFirst, serverFirst);
            message.SetProof("bf45fcbf7073d93d022466c94321745fe1c8e13b");

            message.Channel.Header.ShouldBe("biws");
            message.Nonce.Value.ShouldBe("nonce");
            message.Proof?.ToString().ShouldBe("p=bf45fcbf7073d93d022466c94321745fe1c8e13b");
            message.Message.ShouldBe("c=biws,r=nonce,p=bf45fcbf7073d93d022466c94321745fe1c8e13b");
            message.MessageWithoutProof.ShouldBe("c=biws,r=nonce");
        }
    }
}
