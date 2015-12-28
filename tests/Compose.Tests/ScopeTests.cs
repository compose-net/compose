using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using TestAttributes;

namespace Compose.Tests
{
    public class ScopeTests
    {
        [Unit]
        public void GivenATypeBasedServiceDescriptorWhenResolvingTransientThenProviderReturnsMultipleInstances()
        {
            var app = new Fake.FakeExecutable();
            app.UseServices(services => services.AddTransient<Fake.Service, Fake.Implementation>());
            app.CanResolveMultipleInstances<Fake.Service>().Should().BeTrue();
        }

        [Unit]
        public void GivenATypeBasedServiceDescriptorWhenResolvingTransientTransitionalThenProviderReturnsMultipleInstances()
        {
            var app = new Fake.FakeExecutable();
            app.UseServices(services => services.AddTransient<Fake.Service, Fake.Implementation>());
            app.UseProvider<Fake.Service>(services => services.AddTransient<Fake.Service, Fake.AlternativeImplementation>());
            app.CanResolveMultipleInstances<Fake.Service>().Should().BeTrue();
        }

        [Unit]
        public void GivenATypeBasedServiceDescriptorWhenResolvingSingletonThenProviderReturnsSingleInstance()
        {
            var app = new Fake.FakeExecutable();
            app.UseServices(services => services.AddSingleton<Fake.Service, Fake.Implementation>());
            app.CanResolveMultipleInstances<Fake.Service>().Should().BeFalse();
        }

        [Unit]
        public void GivenATypeBasedServiceDescriptorWhenResolvingSingletonTransitionalThenProviderReturnsSingleInstance()
        {
            var app = new Fake.FakeExecutable();
            app.UseServices(services => services.AddSingleton<Fake.Service, Fake.Implementation>());
            app.UseProvider<Fake.Service>(services => services.AddTransient<Fake.Service, Fake.AlternativeImplementation>());
            app.CanResolveMultipleInstances<Fake.Service>().Should().BeFalse();
        }

        [Unit]
        public void GivenAInstanceBasedServiceDescriptorWhenResolvingInstanceThenProviderReturnsSingleInstance()
        {
            var app = new Fake.FakeExecutable();
            var instance = new Fake.Implementation();
            app.UseServices(services => services.AddInstance<Fake.Service>(instance));
            app.CanResolveMultipleInstances<Fake.Service>().Should().BeFalse();
        }

        [Unit]
        public void GivenAInstanceBasedServiceDescriptorWhenResolvingTransitionalInstanceThenProviderReturnsSingleInstance()
        {
            var app = new Fake.FakeExecutable();
            var instance = new Fake.Implementation();
            app.UseServices(services => services.AddInstance<Fake.Service>(instance));
            app.UseProvider<Fake.Service>(services => services.AddInstance<Fake.Service>(new Fake.AlternativeImplementation()));
            app.CanResolveMultipleInstances<Fake.Service>().Should().BeFalse();
        }

        [Unit]
        public void GivenAFactoryBasedServiceDescriptorWhenTransientFactoryReturnsInstancesThenProviderReturnsInstances()
        {
            var app = new Fake.FakeExecutable();
            Func<IServiceProvider, Fake.Service> factory = provider => new Fake.Implementation();
            app.UseServices(services => services.AddTransient(factory));
            app.CanResolveMultipleInstances<Fake.Service>().Should().BeTrue();
        }

        [Unit]
        public void GivenAFactoryBasedServiceDescriptorWhenTransientTransitionalFactoryReturnsInstancesThenProviderReturnsInstances()
        {
            var app = new Fake.FakeExecutable();
            Func<IServiceProvider, Fake.Service> factory = provider => new Fake.Implementation();
            app.UseServices(services => services.AddTransient(factory));
            app.UseProvider<Fake.Service>(services => services.AddTransient<Fake.Service, Fake.AlternativeImplementation>());
            app.CanResolveMultipleInstances<Fake.Service>().Should().BeTrue();
        }

        [Unit]
        public void GivenAFactoryBasedServiceDescriptorWhenSingletonFactoryReturnsInstancesThenProviderReturnsSingleInstance()
        {
            var app = new Fake.FakeExecutable();
            Func<IServiceProvider, Fake.Service> factory = provider => new Fake.Implementation();
            app.UseServices(services => services.AddSingleton(factory));
            app.CanResolveMultipleInstances<Fake.Service>().Should().BeFalse();
        }

        [Unit]
        public void GivenAFactoryBasedServiceDescriptorWhenSingletonTransitionalFactoryReturnsInstancesThenProviderReturnsSingleInstance()
        {
            var app = new Fake.FakeExecutable();
            Func<IServiceProvider, Fake.Service> factory = provider => new Fake.Implementation();
            app.UseServices(services => services.AddSingleton(factory));
            app.UseProvider<Fake.Service>(services => services.AddTransient<Fake.Service, Fake.AlternativeImplementation>());
            app.CanResolveMultipleInstances<Fake.Service>().Should().BeFalse();
        }
    }
}
