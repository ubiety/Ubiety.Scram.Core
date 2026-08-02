using System;
using System.Text;
using Shouldly;
using Ubiety.Scram.Core;
using Ubiety.Scram.Core.Messages;
using Xunit;

namespace Ubiety.Scram.Test.Messages
{
    /// <summary>
    /// Covers the channel binding declared in the GS2 header of the client first message and the
    /// "c=" attribute it produces in the client final message. RFC 5802 defines that attribute as
    /// base64(gs2-header + cbind-data), with the binding data present only for a "p=" header.
    /// </summary>
    public class ChannelBindingTests
    {
        private const string ServerFirst = "r=fyko+d2lbbFgONRv9qkxdawL3rfcNHYJY1ZVvWVs7j,s=QSXCR+Q6sek8bf92,i=4096";

        private static readonly byte[] Token = Encoding.UTF8.GetBytes("EXPORTER-TOKEN-32-BYTES-EXAMPLE!");

        [Theory]
        [InlineData(TlsVersion.TlsExporter, "p=tls-exporter,,")]
        [InlineData(TlsVersion.TlsUnique, "p=tls-unique,,")]
        [InlineData(TlsVersion.TlsServerEndpoint, "p=tls-server-end-point,,")]
        public void When_BindingIsRequired_ShouldEncodeTheHeaderAndTheToken(TlsVersion version, string header)
        {
            var message = Build(ChannelBindingStatus.Required, version, Token);

            Decode(message).ShouldBe([.. Encoding.UTF8.GetBytes(header), .. Token]);
        }

        [Theory]
        [InlineData(ChannelBindingStatus.NotSupported, "n,,")]
        [InlineData(ChannelBindingStatus.ClientSupport, "y,,")]
        public void When_BindingIsNotInUse_ShouldEncodeTheHeaderAlone(ChannelBindingStatus status, string header)
        {
            var message = Build(status, TlsVersion.TlsUnique, token: null);

            Decode(message).ShouldBe(Encoding.UTF8.GetBytes(header));
        }

        [Fact]
        public void When_BindingIsRequiredWithoutAToken_ShouldThrow()
        {
            // Otherwise the message advertises a binding it does not carry, and the only symptom
            // is the server rejecting a proof that looks correct from the client's side.
            var exception = Should.Throw<ArgumentException>(
                () => Build(ChannelBindingStatus.Required, TlsVersion.TlsExporter, token: null));

            exception.ParamName.ShouldBe("token");
        }

        [Fact]
        public void When_BindingIsRequiredWithAnEmptyToken_ShouldThrow()
        {
            Should.Throw<ArgumentException>(
                () => Build(ChannelBindingStatus.Required, TlsVersion.TlsExporter, []));
        }

        [Theory]
        [InlineData(ChannelBindingStatus.NotSupported)]
        [InlineData(ChannelBindingStatus.ClientSupport)]
        public void When_ATokenIsSuppliedWithoutRequiringBinding_ShouldThrow(ChannelBindingStatus status)
        {
            // RFC 5802 allows cbind-data only behind a "p=" header, so appending it to "n,," or
            // "y,," produces a message no conforming server accepts.
            var exception = Should.Throw<ArgumentException>(
                () => Build(status, TlsVersion.TlsUnique, Token));

            exception.ParamName.ShouldBe("token");
        }

        [Fact]
        public void When_TheHeaderIsRewrittenInFlight_TheProofShouldChange()
        {
            // The GS2 header is signed as part of the auth message, which is what lets a server
            // detect a downgrade that strips the binding from the client first message.
            var bound = Build(ChannelBindingStatus.Required, TlsVersion.TlsExporter, Token);
            var stripped = Build(ChannelBindingStatus.ClientSupport, TlsVersion.TlsExporter, token: null);

            bound.Proof?.ToString().ShouldNotBe(stripped.Proof?.ToString());
        }

        private static ClientFinalMessage Build(ChannelBindingStatus status, TlsVersion version, byte[]? token)
        {
            var clientFirst = new ClientFirstMessage("user", "fyko+d2lbbFgONRv9qkxdawL", status, version);

            return new ClientFinalMessage(clientFirst, ServerFirstMessage.Parse(ServerFirst), "pencil", Hash.Sha256(), token);
        }

        private static byte[] Decode(ClientFinalMessage message)
        {
            return Convert.FromBase64String(message.Channel.ToString()["c=".Length..]);
        }
    }
}
