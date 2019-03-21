using Shouldly;
using Ubiety.Scram.Core.Attributes;
using Xunit;

namespace Ubiety.Scram.Test.Attributes
{
    public class UnknownAttributeTests
    {
        [Fact]
        public void When_CreatedWithNameAndValue_ValueShouldBeValid()
        {
            var attribute = new UnknownAttribute('h', "name");

            attribute.Value.ShouldBe("name");
            attribute.ToString().ShouldBe("h=name");
        }
    }
}
