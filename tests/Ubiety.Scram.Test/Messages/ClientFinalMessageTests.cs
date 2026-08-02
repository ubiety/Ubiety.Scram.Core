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
            var serverFirst = new ServerFirstMessage(4096, "nonceserver", "salt", "");

            var message = new ClientFinalMessage(clientFirst, serverFirst, "", Hash.Sha1());

            message.Channel.ToString().ShouldBe("c=biws");
            message.Nonce.Value.ShouldBe("nonceserver");
            message.Proof?.ToString().ShouldBe("p=gq2GOv361cqXnm+fQm64zzWqYeI=");
            message.Message.ShouldBe("c=biws,r=nonceserver,p=gq2GOv361cqXnm+fQm64zzWqYeI=");
            message.MessageWithoutProof.ShouldBe("c=biws,r=nonceserver");
        }

        [Fact]
        public void When_ProofIsCalculated_TheServerSignatureShouldBeBase64()
        {
            var clientFirst = new ClientFirstMessage("user", "nonce");
            var serverFirst = new ServerFirstMessage(4096, "nonceserver", "QSXCR+Q6sek8bf92", "r=nonceserver,s=QSXCR+Q6sek8bf92,i=4096");

            var message = new ClientFinalMessage(clientFirst, serverFirst, "pencil", Hash.Sha256());

            var decoded = Convert.FromBase64String(message.ServerSignature);
            decoded.Length.ShouldBe(32);

            // The signature the client computes has to compare equal to the one the
            // server sends back in its final message.
            var serverFinal = ServerFinalMessage.Parse($"v={message.ServerSignature}");
            (serverFinal.ServerSignature == message.ServerSignature).ShouldBeTrue();
        }

        [Theory]
        [InlineData("evilnonce")]      // Ignores the client nonce entirely.
        [InlineData("noncf-server")]   // Alters the client's part before appending.
        [InlineData("onceserver")]     // Contains the client nonce, but not as a prefix.
        [InlineData("nonce")]          // Echoes the client nonce with nothing appended.
        [InlineData("")]               // Omits the nonce.
        public void When_ServerNonceDoesNotExtendTheClientNonce_ItShouldThrow(string serverNonce)
        {
            var clientFirst = new ClientFirstMessage("user", "nonce");
            var serverFirst = new ServerFirstMessage(4096, serverNonce, "salt", "");

            Should.Throw<ArgumentException>(
                () => new ClientFinalMessage(clientFirst, serverFirst, "pencil", Hash.Sha256()));
        }

        [Fact]
        public void When_ServerNonceExtendsTheClientNonce_ItShouldBeUsedWhole()
        {
            var clientFirst = new ClientFirstMessage("user", "fyko+d2lbbFgONRv9qkxdawL");
            var serverFirst = new ServerFirstMessage(
                4096,
                "fyko+d2lbbFgONRv9qkxdawL3rfcNHYJY1ZVvWVs7j",
                "QSXCR+Q6sek8bf92",
                "r=fyko+d2lbbFgONRv9qkxdawL3rfcNHYJY1ZVvWVs7j,s=QSXCR+Q6sek8bf92,i=4096");

            var message = new ClientFinalMessage(clientFirst, serverFirst, "pencil", Hash.Sha256());

            message.Nonce.Value.ShouldBe("fyko+d2lbbFgONRv9qkxdawL3rfcNHYJY1ZVvWVs7j");
        }

        [Theory]
        [InlineData(1)]
        [InlineData(4095)]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(ClientFinalMessage.MaximumIterations + 1)]
        public void When_IterationCountIsOutOfRange_ItShouldThrow(int iterations)
        {
            var clientFirst = new ClientFirstMessage("user", "nonce");
            var serverFirst = new ServerFirstMessage(iterations, "nonceserver", "salt", "");

            Should.Throw<ArgumentException>(
                () => new ClientFinalMessage(clientFirst, serverFirst, "pencil", Hash.Sha256()));
        }

        [Fact]
        public void When_MinimumIterationsIsLowered_ALegacyCountShouldBeAccepted()
        {
            var clientFirst = new ClientFirstMessage("user", "nonce");
            var serverFirst = new ServerFirstMessage(1000, "nonceserver", "salt", "");

            var message = new ClientFinalMessage(
                clientFirst, serverFirst, "pencil", Hash.Sha256(), token: null, minimumIterations: 1000);

            message.Proof.ShouldNotBeNull();
        }
    }
}
