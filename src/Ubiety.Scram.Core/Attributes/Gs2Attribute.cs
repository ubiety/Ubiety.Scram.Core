// This is free and unencumbered software released into the public domain.
//
// Anyone is free to copy, modify, publish, use, compile, sell, or
// distribute this software, either in source code form or as a compiled
// binary, for any purpose, commercial or non-commercial, and by any
// means.
//
// In jurisdictions that recognize copyright laws, the author or authors
// of this software dedicate any and all copyright interest in the
// software to the public domain. We make this dedication for the benefit
// of the public at large and to the detriment of our heirs and
// successors. We intend this dedication to be an overt act of
// relinquishment in perpetuity of all present and future rights to this
// software under copyright law.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// For more information, please refer to <http://unlicense.org/>

using System;

namespace Ubiety.Scram.Core.Attributes
{
    /// <summary>
    /// Represents the GS2 attribute used in the SCRAM (Salted Challenge Response
    /// Authentication Mechanism) protocol to indicate the channel binding status.
    /// </summary>
    public class Gs2Attribute : ScramAttribute
    {
        /// <summary>
        /// The prefix a gs2-cbind-flag carries when it names a channel binding type.
        /// </summary>
        internal const string BindingTypePrefix = "p=";

        private const string TlsExporterName = "tls-exporter";
        private const string TlsUniqueName = "tls-unique";
        private const string TlsServerEndpointName = "tls-server-end-point";

        /// <summary>
        ///     Initializes a new instance of the <see cref="Gs2Attribute"/> class.
        /// </summary>
        /// <param name="bindingStatus">Channel binding status.</param>
        /// <param name="version">TLS version of the socket.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when either value is not one the enum defines. Rejecting it here keeps the
        /// header this attribute renders honest, rather than falling back to a flag the caller
        /// did not ask for.
        /// </exception>
        public Gs2Attribute(ChannelBindingStatus bindingStatus, TlsVersion version)
            : base('p')
        {
            if (!Enum.IsDefined(bindingStatus))
            {
                throw new ArgumentOutOfRangeException(nameof(bindingStatus), bindingStatus, "Unknown channel binding status.");
            }

            if (!Enum.IsDefined(version))
            {
                throw new ArgumentOutOfRangeException(nameof(version), version, "Unknown channel binding type.");
            }

            ChannelBindingStatus = bindingStatus;
            Version = version;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Gs2Attribute"/> class.
        /// </summary>
        /// <param name="header">
        /// The gs2-cbind-flag as it appeared on the wire: "n", "y", or "p=&lt;cb-name&gt;", without
        /// the trailing separators.
        /// </param>
        /// <exception cref="FormatException">
        /// Thrown when the flag is empty, is not one RFC 5802 defines, or names a channel binding
        /// type this library does not implement.
        /// </exception>
        public Gs2Attribute(string header)
            : base('p')
        {
            if (string.IsNullOrEmpty(header))
            {
                throw new FormatException("A GS2 header cannot be empty.");
            }

            // The whole flag has to be recognised, not just its first character. Falling back to
            // "no binding requested" would let a tampered or unreadable flag read as a client that
            // never asked for binding, which is the downgrade the flag exists to make detectable.
            ChannelBindingStatus = header switch
            {
                "n" => ChannelBindingStatus.NotSupported,
                "y" => ChannelBindingStatus.ClientSupport,
                _ when header.StartsWith(BindingTypePrefix, StringComparison.Ordinal) => ChannelBindingStatus.Required,
                _ => throw new FormatException(
                    $"'{header}' is not a GS2 channel binding flag. RFC 5802 defines 'n', 'y', and 'p=<cb-name>'."),
            };

            // A "p=<cb-name>" flag also names the binding, which has to survive the round trip so
            // ToString rebuilds the header the peer actually sent. A name with no TlsVersion to
            // hold it could not survive, so it is rejected rather than quietly rewritten to a
            // different binding type.
            Version = ChannelBindingStatus == ChannelBindingStatus.Required
                ? ParseBindingType(header)
                : TlsVersion.TlsUnique;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Gs2Attribute"/> class.
        /// </summary>
        public Gs2Attribute()
            : base('p')
        {
        }

        /// <summary>
        /// Gets the channel binding status in the SCRAM (Salted Challenge Response Authentication Mechanism) protocol.
        /// </summary>
        public ChannelBindingStatus ChannelBindingStatus { get; internal init; }

        /// <summary>
        /// Gets the TLS version of the socket.
        /// </summary>
        public TlsVersion Version { get; internal init; }

        /// <summary>
        /// Converts the specified <see cref="Gs2Attribute"/> instance to its string representation.
        /// </summary>
        /// <param name="attribute">The GS2 attribute to be converted into a string.</param>
        /// <returns>A string representation of the GS2 attribute.</returns>
        public static implicit operator string(Gs2Attribute attribute)
        {
            return attribute.ToString();
        }

        /// <summary>
        /// Returns the string representation of the <see cref="Gs2Attribute"/> object
        /// based on its current channel binding status.
        /// </summary>
        /// <returns>
        /// A string that corresponds to the <see cref="ChannelBindingStatus"/> value of the object.
        /// </returns>
        public override string ToString()
        {
            // Both constructors reject a status the enum does not define, so the final arm only
            // ever covers NotSupported. It is written as the default because ToString must not
            // throw, not because an unrecognised status should render as "no binding requested".
            return ChannelBindingStatus switch
            {
                ChannelBindingStatus.ClientSupport => "y,,",
                ChannelBindingStatus.Required => $"{BindingTypePrefix}{BindingTypeName(Version)},,",
                _ => "n,,",
            };
        }

        /// <summary>
        /// Maps the cb-name in a "p=" flag to the binding type it names.
        /// </summary>
        /// <param name="header">The whole flag, including its "p=" prefix.</param>
        /// <returns>The binding type.</returns>
        /// <exception cref="FormatException">Thrown when the name is not one this library implements.</exception>
        private static TlsVersion ParseBindingType(string header)
        {
            return header switch
            {
                BindingTypePrefix + TlsExporterName => TlsVersion.TlsExporter,
                BindingTypePrefix + TlsUniqueName => TlsVersion.TlsUnique,
                BindingTypePrefix + TlsServerEndpointName => TlsVersion.TlsServerEndpoint,
                _ => throw new FormatException(
                    $"'{header[BindingTypePrefix.Length..]}' is not a channel binding type this library " +
                    "implements. RFC 5802 requires a peer that does not support the named type to reject " +
                    "the exchange rather than continue over a different one."),
            };
        }

        /// <summary>
        /// Names a binding type the way it appears in a "p=" flag.
        /// </summary>
        /// <param name="version">The binding type.</param>
        /// <returns>The cb-name, without the "p=" prefix.</returns>
        private static string BindingTypeName(TlsVersion version)
        {
            return version switch
            {
                TlsVersion.TlsExporter => TlsExporterName,
                TlsVersion.TlsUnique => TlsUniqueName,
                TlsVersion.TlsServerEndpoint => TlsServerEndpointName,
                _ => throw new InvalidOperationException($"'{version}' is not a channel binding type."),
            };
        }
    }
}
