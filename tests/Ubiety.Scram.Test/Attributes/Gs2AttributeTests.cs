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

        [Theory]
        [InlineData("z")]           // Not a flag RFC 5802 defines.
        [InlineData("nonsense")]    // Starts with 'n', but is not "n".
        [InlineData("yes")]         // Starts with 'y', but is not "y".
        [InlineData("N")]           // The flags are case sensitive.
        public void When_TheFlagIsNotOneRfc5802Defines_ShouldThrowAFormatException(string header)
        {
            // Falling back to NotSupported would read a tampered or unreadable flag as a client
            // that never asked for binding, which is the downgrade the flag exists to expose.
            Should.Throw<FormatException>(() => new Gs2Attribute(header));
        }

        [Theory]
        [InlineData("p=tls-future")]
        [InlineData("p=")]
        [InlineData("p=TLS-UNIQUE")]
        public void When_TheBindingTypeIsUnknown_ShouldThrowAFormatException(string header)
        {
            // Quietly reading it as tls-unique would rewrite the header on the way back out, so
            // the reconstructed message would no longer match the one the peer signed.
            Should.Throw<FormatException>(() => new Gs2Attribute(header));
        }

        [Theory]
        [InlineData("n")]
        [InlineData("y")]
        [InlineData("p=tls-exporter")]
        [InlineData("p=tls-unique")]
        [InlineData("p=tls-server-end-point")]
        public void When_TheFlagIsValid_ShouldRoundTripToTheSameHeader(string header)
        {
            new Gs2Attribute(header).ToString().ShouldBe($"{header},,");
        }

        [Fact]
        public void When_TheBindingStatusIsNotDefined_ShouldThrow()
        {
            Should.Throw<ArgumentOutOfRangeException>(
                () => new Gs2Attribute((ChannelBindingStatus)99, TlsVersion.TlsUnique));
        }

        [Fact]
        public void When_TheBindingTypeIsNotDefined_ShouldThrow()
        {
            Should.Throw<ArgumentOutOfRangeException>(
                () => new Gs2Attribute(ChannelBindingStatus.Required, (TlsVersion)99));
        }
    }
}
