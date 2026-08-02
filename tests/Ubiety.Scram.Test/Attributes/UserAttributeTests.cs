using System;
using Shouldly;
using Ubiety.Scram.Core.Attributes;
using Ubiety.Stringprep.Core.Exceptions;
using Xunit;

namespace Ubiety.Scram.Test.Attributes
{
    public class UserAttributeTests
    {
        [Fact]
        public void When_UsernameContainsEquals_ShouldBeReplaced()
        {
            var user = new UserAttribute("user=");

            user.ToString().ShouldBe("n=user=3D");
        }

        [Fact]
        public void When_UsernameContainsComma_ShouldBeReplaced()
        {
            var user = new UserAttribute("us,er");

            user.ToString().ShouldBe("n=us=2Cer");
        }

        [Fact]
        public void When_UsernameContainsCommaFromWire_ShouldBeReplaced()
        {
            var user = new UserAttribute("us=2Cer", true);

            user.Value.ShouldBe("us,er");
        }

        [Fact]
        public void When_UsernameContainsEqualsFromWire_ShouldBeReplaced()
        {
            var user = new UserAttribute("user=3D", true);

            user.Value.ShouldBe("user=");
        }

        [Fact]
        public void When_UsernameContainsInvalidEscapedCharacter_ShouldThrowFormatException()
        {
            Should.Throw<FormatException>(() =>
            {
                var _ = new UserAttribute("user=5F", true);
            });
        }

        [Theory]
        [InlineData("user=")]
        [InlineData("user=3")]
        public void When_UsernameContainsTruncatedEscapeSequence_ShouldThrowFormatException(string value)
        {
            Should.Throw<FormatException>(() =>
            {
                var _ = new UserAttribute(value, true);
            });
        }

        [Theory]
        [InlineData("I\u00ADX", "IX")]   // Soft hyphen maps to nothing.
        [InlineData("\u2168", "IX")]     // Roman numeral nine normalizes to ASCII.
        [InlineData("\u00AA", "a")]
        [InlineData("user", "user")]
        public void When_Created_TheUsernameShouldBeSaslPrepped(string input, string expected)
        {
            var user = new UserAttribute(input);

            user.Value.ShouldBe(expected);
            user.ToString().ShouldBe($"n={expected}");
        }

        [Fact]
        public void When_UsernameContainsProhibitedCharacters_ShouldThrow()
        {
            Should.Throw<ProhibitedValueException>(() => new UserAttribute("user\u0007"));
        }

        [Fact]
        public void When_UsernameMixesTextDirections_ShouldThrow()
        {
            Should.Throw<BidirectionalFormatException>(() => new UserAttribute("\u0627\u0031"));
        }

        [Theory]
        [InlineData("I\u00ADX")]         // Would map to nothing under SASLprep.
        [InlineData("\u2168")]           // Would normalize to "IX".
        [InlineData("user\u0007")]       // Prohibited control character.
        [InlineData("\u0627\u0031")]     // Bidirectional violation.
        public void When_AWireUsernameIsNotPrepped_ShouldThrowFormatException(string value)
        {
            // FormatException is what the message TryParse methods treat as a parse failure, so a
            // peer cannot throw a stringprep exception out of TryParse.
            Should.Throw<FormatException>(() => new UserAttribute(value, true));
        }

        [Fact]
        public void When_AWireUsernameIsAlreadyPrepped_ItShouldBeKeptByteForByte()
        {
            // The peer signed its proof over these exact bytes, so parsing must not rewrite them.
            var user = new UserAttribute("IX", true);

            user.Value.ShouldBe("IX");
        }
    }
}
