using Shouldly;
using Ubiety.Scram.Core;
using Ubiety.Scram.Core.Messages;
using Xunit;

namespace Ubiety.Scram.Test.Messages
{
    /// <summary>
    /// Drives the complete SCRAM-SHA-256 exchange from RFC 7677 section 3. The SHA-1 exchange in
    /// <see cref="Rfc5802ExchangeTests"/> is checked the same way, but SHA-256 is the mechanism
    /// most servers actually negotiate, and self-consistent assertions would not catch a
    /// derivation that is wrong on both sides of the comparison.
    /// </summary>
    public class Rfc7677ExchangeTests
    {
        private const string ClientFirst = "n,,n=user,r=rOprNGfwEbeRWgbNEkqO";

        private const string ServerFirst =
            "r=rOprNGfwEbeRWgbNEkqO%hvYDpWUa2RaTCAfuxFIlj)hNlF$k0,s=W22ZaJ0SNY7soEsUEjb6gQ==,i=4096";

        private const string ClientFinal =
            "c=biws,r=rOprNGfwEbeRWgbNEkqO%hvYDpWUa2RaTCAfuxFIlj)hNlF$k0,p=dHzbZapWIk4jUhN+Ute9ytag9zjfMHgsqmmiz7AndVQ=";

        private const string ServerFinal = "v=6rriTRBi23WpRR/wtup+mMhUZUn/dB5nLTJRsjl95G4=";

        [Fact]
        public void When_BuildingTheClientFirstMessage_ShouldMatchTheSpecification()
        {
            var message = new ClientFirstMessage("user", "rOprNGfwEbeRWgbNEkqO");

            message.Message.ShouldBe(ClientFirst);
        }

        [Fact]
        public void When_BuildingTheClientFinalMessage_ShouldMatchTheSpecification()
        {
            Exchange().Message.ShouldBe(ClientFinal);
        }

        [Fact]
        public void When_TheServerRepliesWithItsSignature_ShouldVerify()
        {
            var final = Exchange();

            (ServerFinalMessage.Parse(ServerFinal).ServerSignature == final.ServerSignature).ShouldBeTrue();
        }

        [Fact]
        public void When_TheServerSignatureDoesNotMatch_ShouldNotVerify()
        {
            var final = Exchange();

            (ServerFinalMessage.Parse("v=7rriTRBi23WpRR/wtup+mMhUZUn/dB5nLTJRsjl95G4=").ServerSignature
                == final.ServerSignature).ShouldBeFalse();
        }

        private static ClientFinalMessage Exchange()
        {
            return new ClientFinalMessage(
                ClientFirstMessage.Parse(ClientFirst),
                ServerFirstMessage.Parse(ServerFirst),
                "pencil",
                Hash.Sha256());
        }
    }
}
