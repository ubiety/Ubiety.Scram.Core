using System;
using System.Text;
using Shouldly;
using Ubiety.Scram.Core.Attributes;
using Xunit;

namespace Ubiety.Scram.Test.Attributes
{
    public class ChannelAttributeTests
    {
        [Fact]
        public void When_ThereIsNoToken_ShouldEncodeTheHeaderAlone()
        {
            var attribute = new ChannelAttribute("n,,");

            attribute.ToString().ShouldBe("c=biws");
        }

        [Fact]
        public void When_ConvertedToAString_ShouldUseTheWireFormat()
        {
            ChannelAttribute attribute = new("n,,");

            string converted = attribute;

            converted.ShouldBe("c=biws");
        }

        [Fact]
        public void When_ThereIsAToken_ShouldEncodeTheHeaderFollowedByTheToken()
        {
            var token = new byte[] { 1, 2, 3 };

            var attribute = new ChannelAttribute("p=tls-unique,,", token);

            var expected = Convert.ToBase64String([.. Encoding.UTF8.GetBytes("p=tls-unique,,"), .. token]);
            attribute.ToString().ShouldBe($"c={expected}");
        }
    }
}
