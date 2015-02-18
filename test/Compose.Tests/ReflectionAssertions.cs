using Xunit;

namespace Compose.Tests
{
    public class ReflectionAssertions
    {
		[Fact]
		public void CanResolveMicrosoftFrameworkDependencyInjectionServiceProvider()
		{
			Assert.NotNull(Constants.GetServiceProvider());
		}
    }
}
