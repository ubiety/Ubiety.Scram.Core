using Shouldly;
using Ubiety.Scram.Core.Attributes;
using Xunit;

namespace Ubiety.Scram.Test.Attributes
{
    public class AuthorizationIdentityAttributeTests
    {
        [Fact]
        public void When_CreatedWithName_ValueShouldBeValid()
        {
            var attribute = new AuthorizationIdentityAttribute("user");

            attribute.Value.ShouldBe("user");
        }
    }
}
