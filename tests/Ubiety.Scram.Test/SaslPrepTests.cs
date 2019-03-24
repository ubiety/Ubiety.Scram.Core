using Shouldly;
using Ubiety.Scram.Core;
using Ubiety.Stringprep.Core.Exceptions;
using Xunit;

namespace Ubiety.Scram.Test
{
    public class SaslPrepTests
    {
        [Theory]
        [InlineData("I\u00ADX", "IX")]
        [InlineData("\u00AA", "a")]
        [InlineData("user", "user")]
        [InlineData("USER", "USER")]
        [InlineData("\u2168", "IX")]
        public void When_PassedUnicode_ShouldOutputCharacters(string input, string result)
        {
            var output = SaslPrep.Run(input);

            output.ShouldBe(result);
        }

        [Fact]
        public void When_PassedInvalidCharacters_ShouldThrowAnException()
        {
            Should.Throw<ProhibitedValueException>(() =>
            {
                var _ = SaslPrep.Run("\u0007");
            });
        }

        [Fact]
        public void When_PassedBidiCharacters_ShouldThrowAnException()
        {
            Should.Throw<BidirectionalFormatException>(() =>
            {
                var _ = SaslPrep.Run("\u0627\u0031");
            });
        }
    }
}
