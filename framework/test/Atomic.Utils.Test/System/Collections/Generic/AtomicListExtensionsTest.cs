using Shouldly;
using Xunit;

namespace System.Collections.Generic
{
    public class AtomicListExtensionsTest
    {
        [Fact]
        public void Should_Move_Item_Test()
        {
            var list = new List<int> { 1, 2, 3, 4, 5 };

            // move successfully
            list.MoveItem(num => num == 4, 4);
            list[3].ShouldBe(5);
            list[4].ShouldBe(4);

            // do not move if it stays the same
            list[0].ShouldBe(1);
            list.MoveItem(num => num == 1, 0);
            list[0].ShouldBe(1);

            // throw if it is out of range
            var exception = Should.Throw<IndexOutOfRangeException>(() => list.MoveItem(null, 8));
            exception.Message.ShouldBe($"targetIndex should be between 0 and {list.Count - 1}");
        }
    }
}