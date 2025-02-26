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

namespace Ubiety.Scram.Core.Attributes
{
    /// <summary>
    /// Represents the GS2 attribute used in the SCRAM (Salted Challenge Response
    /// Authentication Mechanism) protocol to indicate the channel binding status.
    /// </summary>
    public class Gs2Attribute : ScramAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Gs2Attribute"/> class.
        /// </summary>
        /// <param name="bindingStatus">Channel binding status.</param>
        public Gs2Attribute(ChannelBindingStatus bindingStatus)
            : base('p')
        {
            ChannelBindingStatus = bindingStatus;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Gs2Attribute"/> class.
        /// </summary>
        /// <param name="header">String version of the header.</param>
        public Gs2Attribute(string header)
            : base('p')
        {
            ChannelBindingStatus = header[0] switch
            {
                'n' => ChannelBindingStatus.NotSupported,
                'y' => ChannelBindingStatus.ClientSupport,
                'p' => ChannelBindingStatus.Required,
                _ => ChannelBindingStatus.NotSupported
            };
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
        public ChannelBindingStatus ChannelBindingStatus { get; }

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
            return ChannelBindingStatus switch
            {
                ChannelBindingStatus.ClientSupport => "y,,",
                ChannelBindingStatus.NotSupported => "n,,",
                ChannelBindingStatus.Required => "p=tls-unique,,",
                _ => "n,,"
            };
        }
    }
}
