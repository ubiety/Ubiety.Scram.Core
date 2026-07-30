using Shouldly;
using Ubiety.Scram.Core;
using Ubiety.Scram.Core.Messages;
using Xunit;

namespace Ubiety.Scram.Test.Messages
{
    /// <summary>
    /// Drives the complete SCRAM-SHA-1 exchange from RFC 5802 section 5 so that every
    /// message the library produces is checked against the specification's own values.
    /// </summary>
    public class Rfc5802ExchangeTests
    {
        private const string ClientFirst = "n,,n=user,r=fyko+d2lbbFgONRv9qkxdawL";
        private const string ServerFirst = "r=fyko+d2lbbFgONRv9qkxdawL3rfcNHYJY1ZVvWVs7j,s=QSXCR+Q6sek8bf92,i=4096";
        private const string ClientFinal = "c=biws,r=fyko+d2lbbFgONRv9qkxdawL3rfcNHYJY1ZVvWVs7j,p=v0X8v3Bz2T0CJGbJQyF0X+HI4Ts=";
        private const string ServerFinal = "v=rmF9pqV8S7suAoZWja4dJRkFsKQ=";

        [Fact]
        public void When_BuildingTheClientFirstMessage_ShouldMatchTheSpecification()
        {
            var message = new ClientFirstMessage("user", "fyko+d2lbbFgONRv9qkxdawL");

            message.Message.ShouldBe(ClientFirst);
        }

        [Fact]
        public void When_BuildingTheClientFinalMessage_ShouldMatchTheSpecification()
        {
            var final = Exchange();

            final.Message.ShouldBe(ClientFinal);
        }

        [Fact]
        public void When_TheServerRepliesWithItsSignature_ShouldVerify()
        {
            var final = Exchange();

            final.ServerSignature.ShouldBe("rmF9pqV8S7suAoZWja4dJRkFsKQ=");
            (ServerFinalMessage.Parse(ServerFinal).ServerSignature == final.ServerSignature).ShouldBeTrue();
        }

        [Fact]
        public void When_TheServerSignatureDoesNotMatch_ShouldNotVerify()
        {
            var final = Exchange();

            (ServerFinalMessage.Parse("v=cmF9pqV8S7suAoZWja4dJRkFsKQ=").ServerSignature == final.ServerSignature)
                .ShouldBeFalse();
        }

        private static ClientFinalMessage Exchange()
        {
            return new ClientFinalMessage(
                ClientFirstMessage.Parse(ClientFirst),
                ServerFirstMessage.Parse(ServerFirst),
                "pencil",
                Hash.Sha1());
        }
    }
}
