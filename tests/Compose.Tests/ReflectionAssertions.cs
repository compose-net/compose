using Microsoft.Framework.DependencyInjection;
using System;
using System.Linq;
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

		[Fact]
		public void CanActivateMicrosoftFrameworkDepdencyInjectionServiceProvider()
		{
			Action act = () => Activator.CreateInstance(Constants.GetServiceProvider(), Enumerable.Empty<ServiceDescriptor>());
			Assert.Null(Record.Exception(act));
        }

		[Fact]
		public void CanActivateServiceProviderThroughWrappingFacade()
		{
			Action act = () => new WrappedServiceProvider(new ServiceCollection());
			Assert.Null(Record.Exception(act));
		}
    }
}
