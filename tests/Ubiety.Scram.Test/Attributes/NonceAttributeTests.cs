using Shouldly;
using Ubiety.Scram.Core.Attributes;
using Xunit;

namespace Ubiety.Scram.Test.Attributes
{
    public class NonceAttributeTests
    {
        [Fact]
        public void When_NonceCreatedWithClientAndServerNonces_ValueShouldBeValid()
        {
            var nonce = new NonceAttribute("client", "server");

            nonce.Value.ShouldBe("clientserver");
        }
    }
}
