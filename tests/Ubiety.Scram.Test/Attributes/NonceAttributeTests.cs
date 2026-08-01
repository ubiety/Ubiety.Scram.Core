using System;
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

        [Fact]
        public void When_TheNonceIsNull_ShouldThrow()
        {
            // A missing nonce reaches here when a server message parsed without an "r=", and a
            // null nonce would otherwise travel into the auth message as an empty string.
            Should.Throw<ArgumentNullException>(() => new NonceAttribute(null));
        }
    }
}
