using System.Text;
using Shouldly;
using Ubiety.Scram.Core.Exceptions;
using Ubiety.Scram.Core.Messages;
using Xunit;

namespace Ubiety.Scram.Test.Messages
{
    /// <summary>
    /// Covers the parsing contract shared by the message types: what <c>Parse</c> throws, what
    /// <c>TryParse</c> returns instead, and the implicit conversions that let a caller hand the
    /// messages straight to a transport.
    /// </summary>
    public class MessageParsingTests
    {
        private const string ValidClientFirst = "n,,n=user,r=fyko+d2lbbFgONRv9qkxdawL";
        private const string ValidServerFirst = "r=fyko+d2lbbFgONRv9qkxdawL3rfcNHYJY1ZVvWVs7j,s=QSXCR+Q6sek8bf92,i=4096";
        private const string ValidServerFinal = "v=rmF9pqV8S7suAoZWja4dJRkFsKQ=";

        // A message carrying none of the attributes the type requires, and one that is not a
        // SCRAM message at all. Both have to fail rather than produce a half-built message.
        [Theory]
        [InlineData("")]
        [InlineData("n,,")]
        [InlineData("nonsense")]
        [InlineData("s=QSXCR+Q6sek8bf92")]
        public void When_TheClientFirstMessageCannotBeParsed_TryParseShouldReturnFalse(string message)
        {
            ClientFirstMessage.TryParse(message, out _).ShouldBeFalse();
        }

        [Theory]
        [InlineData("")]
        [InlineData("nonsense")]
        [InlineData("r=onlyanonce")]
        [InlineData("s=QSXCR+Q6sek8bf92,i=notanumber")]
        public void When_TheServerFirstMessageCannotBeParsed_TryParseShouldReturnFalse(string message)
        {
            ServerFirstMessage.TryParse(message, out _).ShouldBeFalse();
        }

        [Theory]
        [InlineData("")]
        [InlineData("nonsense")]
        [InlineData("r=notasignature")]
        public void When_TheServerFinalMessageCannotBeParsed_TryParseShouldReturnFalse(string message)
        {
            ServerFinalMessage.TryParse(message, out _).ShouldBeFalse();
        }

        [Fact]
        public void When_TheClientFirstMessageCannotBeParsed_ParseShouldThrow()
        {
            Should.Throw<MessageParseException>(() => ClientFirstMessage.Parse("s=QSXCR+Q6sek8bf92"));
        }

        [Fact]
        public void When_TheServerFirstMessageCannotBeParsed_ParseShouldThrow()
        {
            Should.Throw<MessageParseException>(() => ServerFirstMessage.Parse("r=onlyanonce"));
        }

        [Fact]
        public void When_TheServerFinalMessageCannotBeParsed_ParseShouldThrow()
        {
            Should.Throw<MessageParseException>(() => ServerFinalMessage.Parse("r=notasignature"));
        }

        [Theory]
        [InlineData(ValidClientFirst)]
        [InlineData("n,,n=user,r=nonce")]
        public void When_AValidMessageIsParsed_TryParseShouldReturnTrue(string message)
        {
            ClientFirstMessage.TryParse(message, out var result).ShouldBeTrue();

            result.Message.ShouldBe(message);
        }

        [Fact]
        public void When_AClientMessageIsConvertedToBytes_ShouldBeTheUtf8Message()
        {
            var clientFirst = new ClientFirstMessage("user", "fyko+d2lbbFgONRv9qkxdawL");

            byte[] bytes = clientFirst;

            bytes.ShouldBe(Encoding.UTF8.GetBytes(ValidClientFirst));
        }

        [Fact]
        public void When_AClientFinalMessageIsConvertedToBytes_ShouldBeTheUtf8Message()
        {
            var final = new ClientFinalMessage(
                ClientFirstMessage.Parse(ValidClientFirst),
                ServerFirstMessage.Parse(ValidServerFirst),
                "pencil",
                Ubiety.Scram.Core.Hash.Sha256());

            byte[] bytes = final;

            bytes.ShouldBe(Encoding.UTF8.GetBytes(final.Message));
        }

        [Fact]
        public void When_AStringIsConvertedToAClientFirstMessage_ShouldParseIt()
        {
            ClientFirstMessage message = ValidClientFirst;

            message.Message.ShouldBe(ValidClientFirst);
        }

        [Fact]
        public void When_BytesAreConvertedToAClientFirstMessage_ShouldParseIt()
        {
            ClientFirstMessage message = Encoding.UTF8.GetBytes(ValidClientFirst);

            message.Message.ShouldBe(ValidClientFirst);
        }

        [Fact]
        public void When_AStringIsConvertedToAServerFirstMessage_ShouldParseIt()
        {
            ServerFirstMessage message = ValidServerFirst;

            message.Iterations?.Value.ShouldBe(4096);
        }

        [Fact]
        public void When_BytesAreConvertedToAServerFirstMessage_ShouldParseIt()
        {
            ServerFirstMessage message = Encoding.UTF8.GetBytes(ValidServerFirst);

            message.Iterations?.Value.ShouldBe(4096);
        }

        [Fact]
        public void When_AStringIsConvertedToAServerFinalMessage_ShouldParseIt()
        {
            ServerFinalMessage message = ValidServerFinal;

            message.ServerSignature?.ToString().ShouldBe(ValidServerFinal);
        }

        [Fact]
        public void When_BytesAreConvertedToAServerFinalMessage_ShouldParseIt()
        {
            ServerFinalMessage message = Encoding.UTF8.GetBytes(ValidServerFinal);

            message.ServerSignature?.ToString().ShouldBe(ValidServerFinal);
        }
    }
}
