using Shouldly;
using Xunit;

namespace System.Collections.Generic
{
    public class AtomicCollectionExtensionsTest
    {
        [Fact]
        public void Should_Add_If_Not_Contains_Test()
        {
            var nums = new List<int> { 1, 2, 3 };

            nums.AddIfNotContains(4).ShouldBeTrue();
            nums.AddIfNotContains(1).ShouldBeFalse();
        }
    }
}