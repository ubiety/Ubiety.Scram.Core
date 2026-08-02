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

        [Theory]
        [InlineData("n,,")]
        [InlineData("y,,")]
        [InlineData("p=tls-exporter,,")]
        [InlineData("n,a=other,")]
        public void When_ReadFromTheWireWithoutAToken_ShouldRecoverTheHeader(string header)
        {
            var attribute = ChannelAttribute.FromWire(Convert.ToBase64String(Encoding.UTF8.GetBytes(header)));

            attribute.Header.ShouldBe(header);
            attribute.Token.ShouldBeNull();
        }

        [Fact]
        public void When_ReadFromTheWireWithAToken_ShouldSplitTheHeaderFromTheBindingData()
        {
            // A peer verifying the binding needs the two apart: the header to compare against what
            // the client committed to, the data to compare against the TLS connection's token.
            var token = new byte[] { 1, 2, 3, 0x2C, 4 };
            var encoded = Convert.ToBase64String([.. Encoding.UTF8.GetBytes("p=tls-exporter,,"), .. token]);

            var attribute = ChannelAttribute.FromWire(encoded);

            attribute.Header.ShouldBe("p=tls-exporter,,");
            attribute.Token.ShouldBe(token);
        }

        [Theory]
        [InlineData("n,,")]
        [InlineData("p=tls-exporter,,")]
        public void When_ReadFromTheWire_ShouldReEncodeToTheSameValue(string header)
        {
            var token = header[0] == 'p' ? new byte[] { 9, 8, 7 } : null;
            var wire = new ChannelAttribute(header, token).ToString();

            var round = ChannelAttribute.FromWire(wire["c=".Length..]).ToString();

            // The peer signed its proof over these bytes, so parsing must reproduce them exactly.
            round.ShouldBe(wire);
        }

        [Theory]
        [InlineData("bg==")]        // "n" - no separators at all.
        [InlineData("bixh")]        // "n,a" - only one separator.
        [InlineData("not base64")]
        public void When_TheWireValueIsMalformed_ShouldThrowFormatException(string value)
        {
            Should.Throw<FormatException>(() => ChannelAttribute.FromWire(value));
        }

        [Fact]
        public void When_ParsedFromAMessage_ShouldRecoverTheHeaderAndToken()
        {
            var token = new byte[] { 1, 2, 3 };
            var encoded = Convert.ToBase64String([.. Encoding.UTF8.GetBytes("p=tls-exporter,,"), .. token]);

            var attribute = ScramAttribute.Parse($"c={encoded}").ShouldBeOfType<ChannelAttribute>();

            attribute.Header.ShouldBe("p=tls-exporter,,");
            attribute.Token.ShouldBe(token);
            attribute.ToString().ShouldBe($"c={encoded}");
        }
    }
}
