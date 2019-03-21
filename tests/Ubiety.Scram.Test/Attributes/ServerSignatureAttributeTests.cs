using System;
using Shouldly;
using Ubiety.Scram.Core.Attributes;
using Xunit;

namespace Ubiety.Scram.Test.Attributes
{
    public class ServerSignatureAttributeTests
    {
        [Fact]
        public void When_CreatedByParsing_ValueShouldBeValid()
        {
            var attribute = ScramAttribute.Parse("v=rmF9pqV8S7suAoZWja4dJRkFsKQ=");
            var result = new ServerSignatureAttribute("rmF9pqV8S7suAoZWja4dJRkFsKQ=");

            attribute.ShouldBe(result);
        }

        [Fact]
        public void When_AttributeIsNotNull_ShouldBeEqual()
        {
            var attribute = ScramAttribute.Parse("v=rmF9pqV8S7suAoZWja4dJRkFsKQ=");

            Assert.False(attribute.Equals(null));
        }

        [Fact]
        public void When_AttributeIsComparedToItself_ShouldBeEqual()
        {
            var attribute = ScramAttribute.Parse("v=rmF9pqV8S7suAoZWja4dJRkFsKQ=");

            Assert.True(attribute.Equals(attribute));
        }

        [Fact]
        public void When_CreatedFromArray_ShouldEqualTheArray()
        {
            var signature = Convert.FromBase64String("rmF9pqV8S7suAoZWja4dJRkFsKQ=");
            var attribute = new ServerSignatureAttribute(signature);

            Assert.True(attribute.Equals(signature));
        }

        [Fact]
        public void When_CreatingTwoAttributes_TheHashCodesShouldBeEqual()
        {
            var first = ScramAttribute.Parse("v=rmF9pqV8S7suAoZWja4dJRkFsKQ=");
            var second = new ServerSignatureAttribute("rmF9pqV8S7suAoZWja4dJRkFsKQ=");

            first.GetHashCode().ShouldBe(second.GetHashCode());
        }
    }
}
