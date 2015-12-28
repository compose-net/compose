using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using TestAttributes;

namespace Compose.Tests
{
    public class ResolutionTests
    {
        [Unit]
        public void WhenResolvingUnboundServiceThenThrowsDescriptiveException()
        {
            var app = new Application();

            app.UseServices(services => services.Add(ServiceDescriptor.Transient<Fake.Service, Fake.Implementation>()));

            Action act = () => app.ApplicationServices.GetRequiredService<Fake.OpenGenericService<string>>();
            act.ShouldThrow<InvalidOperationException>();
        }

        [Unit]
        public void WhenMultipleTransientImplementationsRegisteredForGenericSerivceThenCanResolve()
        {
            var app = new Application();

            app.UseServices(services =>
            {
                services.AddTransient<Fake.OpenGenericService<string>, Fake.OpenGenericImplementation<string>>();
                services.AddTransient<Fake.OpenGenericService<int>, Fake.OpenGenericImplementation<int>>();
            });

            Action act = () => app.ApplicationServices.GetRequiredService<Fake.OpenGenericService<string>>();
            act.ShouldNotThrow();
            act = () => app.ApplicationServices.GetRequiredService<Fake.OpenGenericService<int>>();
            act.ShouldNotThrow();
            act = () => app.ApplicationServices.GetRequiredService<Fake.OpenGenericService<object>>();
            act.ShouldThrow<Exception>();
        }

        [Unit]
        public void WhenMultipleSingletonImplementationsRegisteredForGenericSerivceThenCanResolve()
        {
            var app = new Application();

            app.UseServices(services =>
            {
                services.AddSingleton<Fake.OpenGenericService<string>, Fake.OpenGenericImplementation<string>>();
                services.AddSingleton<Fake.OpenGenericService<int>, Fake.OpenGenericImplementation<int>>();
            });

            Action act = () => app.ApplicationServices.GetRequiredService<Fake.OpenGenericService<string>>();
            act.ShouldNotThrow();
            act = () => app.ApplicationServices.GetRequiredService<Fake.OpenGenericService<int>>();
            act.ShouldNotThrow();
            act = () => app.ApplicationServices.GetRequiredService<Fake.OpenGenericService<object>>();
            act.ShouldThrow<Exception>();
        }

        [Unit]
        public void WhenAddingClosedAndOpenGenericTransientImplementationsForGenericServiceThenCanResolve()
        {
            var app = new Application();

            app.UseServices(services =>
            {
                services.AddTransient(typeof(Fake.OpenGenericService<>), typeof(Fake.OpenGenericImplementation<>));
                services.AddTransient<Fake.OpenGenericService<int>, Fake.OpenGenericImplementation<int>>();
            });

            Action act = () => app.ApplicationServices.GetRequiredService<Fake.OpenGenericService<string>>();
            act.ShouldNotThrow();
            act = () => app.ApplicationServices.GetRequiredService<Fake.OpenGenericService<int>>();
            act.ShouldNotThrow();
        }

        [Unit]
        public void WhenAddingClosedAndOpenGenericSingletonImplementationsForGenericServiceThenCanResolve()
        {
            var app = new Application();

            app.UseServices(services =>
            {
                services.AddSingleton(typeof(Fake.OpenGenericService<>), typeof(Fake.OpenGenericImplementation<>));
                services.AddSingleton<Fake.OpenGenericService<int>, Fake.OpenGenericImplementation<int>>();
            });

            Action act = () => app.ApplicationServices.GetRequiredService<Fake.OpenGenericService<string>>();
            act.ShouldNotThrow();
            act = () => app.ApplicationServices.GetRequiredService<Fake.OpenGenericService<int>>();
            act.ShouldNotThrow();
        }

        [Unit]
        public void WhenAddingSameServiceTypeMultipleTimesThenCanResolve()
        {
            var app = new Application();

            Action act = () => app.UseServices(services =>
            {
                services.Add(new ServiceDescriptor(typeof(Fake.Service), typeof(Fake.Implementation)));
                services.Add(new ServiceDescriptor(typeof(Fake.Service), typeof(Fake.Implementation)));
            });
            act.ShouldNotThrow();
        }
    }
}
