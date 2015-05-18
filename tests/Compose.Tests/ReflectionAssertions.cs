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
			Constants.GetServiceProvider().Must().NotBeNull();
		}

		[Fact]
		public void CanActivateMicrosoftFrameworkDepdencyInjectionServiceProvider()
		{
			Action act = () => Activator.CreateInstance(Constants.GetServiceProvider(), Enumerable.Empty<ServiceDescriptor>());
			act.MustNotThrow<Exception>();
        }

		[Fact]
		public void CanActivateServiceProviderThroughWrappingFacade()
		{
			Action act = () => new WrappedServiceProvider(new ServiceCollection());
			act.MustNotThrow<Exception>();
		}
    }
}
