using System;
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
        public void When_ProofIsCalculated_TheServerSignatureShouldBeBase64()
        {
            var clientFirst = new ClientFirstMessage("user", "nonce");
            var serverFirst = new ServerFirstMessage(4096, "nonce", "QSXCR+Q6sek8bf92", "r=nonce,s=QSXCR+Q6sek8bf92,i=4096");

            var message = new ClientFinalMessage(clientFirst, serverFirst, "pencil", Hash.Sha256());

            var decoded = Convert.FromBase64String(message.ServerSignature);
            decoded.Length.ShouldBe(32);

            // The signature the client computes has to compare equal to the one the
            // server sends back in its final message.
            var serverFinal = ServerFinalMessage.Parse($"v={message.ServerSignature}");
            (serverFinal.ServerSignature == message.ServerSignature).ShouldBeTrue();
        }
    }
}
