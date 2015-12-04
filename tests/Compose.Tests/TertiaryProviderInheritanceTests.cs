using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using Xunit;

namespace Compose.Tests
{
    public class TertiaryProviderInheritanceTests
    {
		[Fact]
		public void WhenAddingTertiaryProviderThenProviderContainingTertiaryServicesIsReturned()
		{
			var application = new Application();
			application.UseProvider<Fake.Service>(services => services.AddTransient<Fake.Service, Fake.Implementation>())
				.Should().NotBeNull();
		}

		[Fact]
		public void WhenTertiaryProviderConfiguredToUseInternalProviderThenReturnsProviderContainingService()
		{
			var application = new Application();
			application
				.UseProvider<Fake.Service>(services => services.AddTransient<Fake.Service, Fake.Implementation>())
				.ApplicationServices.GetService<Fake.Service>()
				.Should().NotBeNull();
		}

		[Fact]
		public void WhenTertiaryProviderConfiguredToUseCustomProviderThenApplicationServicesMatchesCustomProvider()
		{
			var provider = new Mock<IServiceProvider>().Object;
			var application = new Application();
			application.UseProvider<Fake.Service>(_ => provider).ApplicationServices
				.Should().Be(provider);
		}

		[Fact]
		public void WhenTertiaryProviderAddedThenDoesNotConflictWithPrimaryServices()
		{
			var application = new Application();
			application.UseServices(services => services.AddTransient<Fake.Service, Fake.Implementation>());
			application.UseProvider<Fake.Service>(services => services.AddTransient<Fake.Service, Fake.AlternativeImplementation>());

			application.ApplicationServices.GetRequiredService<Fake.Service>()
				.Should().BeOfType<Fake.Implementation>();
		}

		[Fact]
		public void WhenTertiaryProviderServiceDependsOnPrimaryServiceThenTertiaryServiceCanBeResolved()
		{
			var application = new Application();
			application.UseServices(services => services.AddTransient<Fake.Service, Fake.Implementation>());
			application
				.UseProvider<Fake.Consumer>(services => services.AddTransient<Fake.Consumer>())
				.ApplicationServices.GetService<Fake.Consumer>()
				.Should().NotBeNull();
		}

		[Fact]
		public void WhenTertiaryProviderInheritsTransientFactoryThenConsumerCanBeResolved()
		{
			var application = new Application();
			application.UseServices(services =>
			{
				services.AddTransient<Fake.Implementation>();
				services.AddTransient<Fake.Service>(provider => provider.GetRequiredService<Fake.Implementation>());
			});
			application
				.UseProvider<Fake.Consumer>(services => services.AddTransient<Fake.Consumer>())
				.ApplicationServices.GetService<Fake.Consumer>().Service
				.Should().NotBeNull();
		}

		[Fact]
		public void WhenTertiaryProviderInheritsScopedFactoryThenConsumerCanBeResolved()
		{
			var application = new Application();
			application.UseServices(services =>
			{
				services.AddTransient<Fake.Implementation>();
				services.AddScoped<Fake.Service>(provider => provider.GetRequiredService<Fake.Implementation>());
			});
			application
				.UseProvider<Fake.Consumer>(services => services.AddTransient<Fake.Consumer>())
				.ApplicationServices.GetService<Fake.Consumer>().Service
				.Should().NotBeNull();
		}

		[Fact]
		public void WhenTertiaryProviderInheritsSingletonFactoryThenSingletonIsHonouredAcrossProviders()
		{
			var application = new Application();
			application.UseServices(services =>
			{
				services.AddTransient<Fake.Implementation>();
				services.AddSingleton<Fake.Service>(provider => provider.GetRequiredService<Fake.Implementation>());
			});
			application
				.UseProvider<Fake.Consumer>(services => services.AddTransient<Fake.Consumer>())
				.ApplicationServices.GetRequiredService<Fake.Consumer>().Service
				.Should().Be(application.ApplicationServices.GetRequiredService<Fake.Service>());
		}

		[Fact]
		public void WhenTertiaryProviderInheritsSingletonTypedThenSingletonIsHonouredAcrossProviders()
		{
			var application = new Application();
			application.UseServices(services => services.AddSingleton<Fake.Service, Fake.Implementation>());
			application
				.UseProvider<Fake.Consumer>(services => services.AddTransient<Fake.Consumer>())
				.ApplicationServices.GetRequiredService<Fake.Consumer>().Service
				.Should().Be(application.ApplicationServices.GetRequiredService<Fake.Service>());
		}
    }
}
