using FluentAssertions;
using Xunit;

namespace Compose.Tests
{
    public class ReflectionAssertions
    {
		[Fact]
		public void CanResolveMicrosoftFrameworkDependencyInjectionServiceProvider()
		{
			Constants.GetServiceProvider().Should().NotBeNull();
		}
    }
}
