using FluentAssertions;
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
			Constants.GetServiceProvider().Should().NotBeNull();
		}

		[Fact]
		public void CanActivateMicrosoftFrameworkDepdencyInjectionServiceProvider()
		{
			Action act = () => Activator.CreateInstance(Constants.GetServiceProvider(), Enumerable.Empty<IServiceDescriptor>());
			act.ShouldNotThrow<Exception>();
        }

		[Fact]
		public void CanActivateServiceProviderThroughWrappingFacade()
		{
			Action act = () => new WrappedServiceProvider(new ServiceCollection());
			act.ShouldNotThrow<Exception>();
		}
    }
}
