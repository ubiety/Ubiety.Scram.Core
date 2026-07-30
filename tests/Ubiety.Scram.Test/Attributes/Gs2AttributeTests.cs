using System;
using Shouldly;
using Ubiety.Scram.Core;
using Ubiety.Scram.Core.Attributes;
using Xunit;

namespace Ubiety.Scram.Test.Attributes
{
    public class Gs2AttributeTests
    {
        [Theory]
        [InlineData("p=tls-exporter", TlsVersion.TlsExporter)]
        [InlineData("p=tls-unique", TlsVersion.TlsUnique)]
        [InlineData("p=tls-server-end-point", TlsVersion.TlsServerEndpoint)]
        public void When_ParsingABindingFlag_ShouldRetainTheTlsVersion(string header, TlsVersion expected)
        {
            var attribute = new Gs2Attribute(header);

            attribute.ChannelBindingStatus.ShouldBe(ChannelBindingStatus.Required);
            attribute.Version.ShouldBe(expected);
            attribute.ToString().ShouldBe($"{header},,");
        }

        [Theory]
        [InlineData("n", ChannelBindingStatus.NotSupported, "n,,")]
        [InlineData("y", ChannelBindingStatus.ClientSupport, "y,,")]
        public void When_ParsingANonBindingFlag_ShouldRoundTrip(string header, ChannelBindingStatus expected, string result)
        {
            var attribute = new Gs2Attribute(header);

            attribute.ChannelBindingStatus.ShouldBe(expected);
            attribute.ToString().ShouldBe(result);
        }

        [Fact]
        public void When_TheHeaderIsEmpty_ShouldThrowAFormatException()
        {
            Should.Throw<FormatException>(() => new Gs2Attribute(string.Empty));
        }
    }
}
