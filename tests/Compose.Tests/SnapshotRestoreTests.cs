using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Compose.Tests
{
	public class SnapshotRestoreTests
	{
		[Fact]
		public static void WhenRestoringImplicitlySnapshottedServiceThenServiceCanBeResolved()
		{
			var application = new Application();
			application.UseServices(services => services.AddTransient<Fake.Service, Fake.Implementation>());
			var provider = application.UseProvider<Fake.Service>(services => services.AddTransient<Fake.Service, Fake.AlternativeImplementation>());
			var service = application.ApplicationServices.GetRequiredService<Fake.Service>();

			service.ServiceType.Should().Be(typeof(Fake.Implementation));
			application.Transition(provider);
			service.ServiceType.Should().Be(typeof(Fake.AlternativeImplementation));
			application.Restore();

			service.ServiceType.Should().Be(typeof(Fake.Implementation));
		}

		[Fact]
		public static void WhenRestoringExplcitlySnapshottedServiceThenServiceCanBeResolved()
		{
			var application = new Application();
			application.UseServices(services => services.AddTransient<Fake.Service, Fake.Implementation>());
			var alternativeProvider = application.UseProvider<Fake.Service>(services => services.AddTransient<Fake.Service, Fake.AlternativeImplementation>());
			var additionalProvider = application.UseProvider<Fake.Service>(services => services.AddTransient<Fake.Service, Fake.AdditionalImplementation>());
			var service = application.ApplicationServices.GetRequiredService<Fake.Service>();

			application.Transition(alternativeProvider);
			service.ServiceType.Should().Be(typeof(Fake.AlternativeImplementation));
			application.Snapshot();
			application.Transition(additionalProvider);
			service.ServiceType.Should().Be(typeof(Fake.AdditionalImplementation));
			application.Restore();

			service.ServiceType.Should().Be(typeof(Fake.AlternativeImplementation));
		}
	}
}
