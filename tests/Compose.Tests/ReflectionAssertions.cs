using Microsoft.Framework.DependencyInjection;
using System;
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

		[Unit]
		public void CanActivateServiceProviderThroughWrappingFacade()
		{
			Action act = () => new WrappedServiceProvider(new ServiceCollection());
			Assert.Null(Record.Exception(act));
		}
    }
}
