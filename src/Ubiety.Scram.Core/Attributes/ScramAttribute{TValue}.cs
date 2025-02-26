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
    /// Represents a generic SCRAM (Salted Challenge Response Authentication Mechanism) attribute with a name and value.
    /// </summary>
    /// <typeparam name="TValue">Type of the attribute value.</typeparam>
    public class ScramAttribute<TValue> : ScramAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ScramAttribute{TValue}"/> class.
        /// </summary>
        /// <param name="name">Attribute name.</param>
        /// <param name="value">Attribute value.</param>
        public ScramAttribute(char name, TValue value)
            : base(name)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the value associated with the SCRAM attribute.
        /// </summary>
        /// <remarks>
        /// The value represents the specific data associated with the attribute and is of a generic type, allowing support for various types of values.
        /// </remarks>
        public TValue Value { get; }

        /// <summary>
        /// Converts the attribute to its string representation.
        /// </summary>
        /// <returns>The string representation of the attribute, including its name and value.</returns>
        public override string ToString()
        {
            return $"{Name}={Value}";
        }
    }
}
