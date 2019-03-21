using System;
using Shouldly;
using Ubiety.Scram.Core.Attributes;
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
    }
}
