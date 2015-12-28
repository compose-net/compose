using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using TestAttributes;

namespace Compose.Tests
{
    public class DisposableTests
    {
        [Unit]
        public void WhenTransitioningAwayFromDirectlyImplementedDisposableThenDisposesCurrentService()
        {
            var app = new Fake.FakeExecutable();
            app.UseServices(services => services.AddTransient<Fake.Service, Fake.DirectlyDisposableImplementation>());
            var provider = app.UseProvider<Fake.Service>(services => services.AddTransient<Fake.Service, Fake.Implementation>());

            app.OnExecute<Fake.Service>(dependency =>
            {
                Action act = () => app.Transition(provider);
                act.ShouldThrow<NotImplementedException>();
                return true;
            });

            app.Execute();
        }

        [Unit]
        public void WhenTransitioningAwayFromIndirectlyImplementedDisposableThenDisposesCurrentService()
        {
            var app = new Fake.FakeExecutable();
            app.UseServices(services => services.AddTransient<Fake.Service, Fake.IndirectlyDisposableImplementation>());
            var provider = app.UseProvider<Fake.Service>(services => services.AddTransient<Fake.Service, Fake.Implementation>());

            app.OnExecute<Fake.Service>(dependency =>
            {
                Action act = () => app.Transition(provider);
                act.ShouldThrow<NotImplementedException>();
                return true;
            });

            app.Execute();
        }

        [Unit]
        public void WhenRestoringAwayFromDirectlyImplementedDisposableThenDisposesCurrentService()
        {
            var app = new Fake.FakeExecutable();
            app.UseServices(services => services.AddTransient<Fake.Service, Fake.Implementation>());
            var provider = app.UseProvider<Fake.Service>(services => services.AddTransient<Fake.Service, Fake.DirectlyDisposableImplementation>());

            app.OnExecute<Fake.Service>(dependency =>
            {
                app.Transition(provider);
                Action act = () => app.Restore();
                act.ShouldThrow<TargetInvocationException>().WithInnerException<NotImplementedException>();
                return true;
            });

            app.Execute();
        }

        [Unit]
        public void WhenRestoringAwayFromIndirectlyImplementedDisposableThenDisposesCurrentService()
        {
            var app = new Fake.FakeExecutable();
            app.UseServices(services => services.AddTransient<Fake.Service, Fake.Implementation>());
            var provider = app.UseProvider<Fake.Service>(services => services.AddTransient<Fake.Service, Fake.IndirectlyDisposableImplementation>());

            app.OnExecute<Fake.Service>(dependency =>
            {
                app.Transition(provider);
                Action act = () => app.Restore();
                act.ShouldThrow<TargetInvocationException>().WithInnerException<NotImplementedException>();
                return true;
            });

            app.Execute();
        }
    }
}