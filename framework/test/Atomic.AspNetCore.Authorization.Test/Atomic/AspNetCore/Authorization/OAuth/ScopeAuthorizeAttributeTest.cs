using System.Reflection;
using Shouldly;
using Xunit;

namespace Atomic.AspNetCore.Authorization.OAuth
{
    public class ScopeAuthorizeAttributeTest
    {
        [Fact]
        public void Should_Get_Policy_From_Attribute()
        {
            var attribute = typeof(AuthorService).GetCustomAttribute<ScopeAuthorizeAttribute>();

            attribute.ShouldNotBeNull();
            attribute.ScopeName.ShouldBe("Author.Get");

            attribute.ScopeName = "Author.Create";
            attribute.Policy.ShouldBe("Scope:Author.Create");
        }

        [ScopeAuthorize("Author.Get")]
        public class AuthorService
        {
        }
    }
}