using System;
using Shouldly;
using Xunit;

namespace Atomic.Utils
{
    public class CheckTest
    {
        [Fact]
        public void Should_Return_Not_Null_Value()
        {
            const string str = "message";
            var checkedStr = Check.NotNull(str, nameof(str));
            checkedStr.ShouldBe(str);
        }

        [Fact]
        public void Should_Throw_If_Check_Null()
        {
            const string str = null;
            var exception = Should.Throw<ArgumentNullException>(() => Check.NotNull(str, nameof(str)));
            exception.ParamName.ShouldBe(nameof(str));
        }
    }
}