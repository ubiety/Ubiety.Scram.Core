using System;
using Shouldly;
using Ubiety.Scram.Core.Attributes;
using Xunit;

namespace Ubiety.Scram.Test.Attributes
{
    public class ScramAttributeTests
    {
        [Fact]
        public void When_NameIsTwoCharacters_ShouldThrowAnException()
        {
            Should.Throw<FormatException>(() =>
            {
                var _ = ScramAttribute.Parse("bh=name");
            });
        }

        [Fact]
        public void When_ThereAreMoreParts_ShouldThrowAnException()
        {
            Should.Throw<FormatException>(() =>
            {
                var _ = ScramAttribute.Parse("value");
            });
        }

        [Fact]
        public void When_ChannelIsNotSupported_ShouldBeValid()
        {
            var _ = ScramAttribute.ParseAll("n,,n=name");
        }

        [Fact]
        public void When_ParsingAnAuthorizationAttribute_ShouldBeValid()
        {
            var attribute = ScramAttribute.Parse("a=name");

            attribute.ShouldBeOfType<AuthorizationIdentityAttribute>();
        }

        [Fact]
        public void When_ParsingAUserAttribute_ShouldBeValid()
        {
            var attribute = ScramAttribute.Parse("n=name");

            attribute.ShouldBeOfType<UserAttribute>();
        }

        [Fact]
        public void When_ParsingAChannelAttribute_ShouldBeValid()
        {
            var attribute = ScramAttribute.Parse("c=biws");

            attribute.ShouldBeOfType<ChannelAttribute>();
        }

        [Fact]
        public void When_ParsingAClientProofAttribute_ShouldBeValid()
        {
            var attribute = ScramAttribute.Parse("p=v0X8v3Bz2T0CJGbJQyF0X+HI4Ts=");

            attribute.ShouldBeOfType<ClientProofAttribute>();
        }

        [Fact]
        public void When_ParsingAnUnknownAttribute_ShouldBeValid()
        {
            var attribute = ScramAttribute.Parse("h=name");

            attribute.ShouldBeOfType<UnknownAttribute>();
        }
    }
}
