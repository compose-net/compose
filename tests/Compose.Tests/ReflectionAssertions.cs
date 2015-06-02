using Microsoft.Framework.DependencyInjection;
using System;
using Xunit;

namespace Compose.Tests
{
    public class ReflectionAssertions
    {
		[Fact]
		public void CanBuildMicrosoftFrameworkDependencyInjectionServiceProvider()
		{
			Assert.NotNull(new ServiceCollection().BuildServiceProvider());
		}

		[Fact]
		public void CanActivateServiceProviderThroughWrappingFacade()
		{
			Action act = () => new WrappedServiceProvider(new ServiceCollection());
			Assert.Null(Record.Exception(act));
		}
    }
}
