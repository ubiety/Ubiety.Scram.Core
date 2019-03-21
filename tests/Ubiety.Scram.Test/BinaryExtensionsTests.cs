using System;
using Shouldly;
using Ubiety.Scram.Core;
using Xunit;

namespace Ubiety.Scram.Test
{
    public class BinaryExtensionsTests
    {
        [Fact]
        public void When_ValuesAreDifferentLength_ExclusiveOrWillThrowAnException()
        {
            Should.Throw<ArgumentException>(() =>
            {
                var one = new byte[] {1, 2, 3, 4, 5};
                var two = new byte[] {1, 2, 3};

                one.ExclusiveOr(two);
            });
        }
    }
}
