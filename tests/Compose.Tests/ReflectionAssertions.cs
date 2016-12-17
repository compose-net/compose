using Microsoft.Extensions.DependencyInjection;
using TestAttributes;
using Xunit;

namespace Compose.Tests
{
	public class ReflectionAssertions
    {
		[Unit]
		public void CanBuildMicrosoftFrameworkDependencyInjectionServiceProvider()
		{
			Assert.NotNull(new ServiceCollection().BuildServiceProvider());
		}
    }
}
