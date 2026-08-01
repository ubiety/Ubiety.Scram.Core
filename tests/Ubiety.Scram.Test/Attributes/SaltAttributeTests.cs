using Shouldly;
using Ubiety.Scram.Core.Attributes;
using Xunit;

namespace Ubiety.Scram.Test.Attributes
{
    public class SaltAttributeTests
    {
        [Fact]
        public void When_ConvertedToAString_ShouldUseTheWireFormat()
        {
            var attribute = new SaltAttribute("QSXCR+Q6sek8bf92");

            attribute.ToString().ShouldBe("s=QSXCR+Q6sek8bf92");
        }
    }
}
