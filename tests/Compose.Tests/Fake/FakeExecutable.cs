using Microsoft.Extensions.DependencyInjection;

namespace Compose.Tests.Fake
{
    internal sealed class FakeExecutable : Executable<bool>
    {
        internal bool CanResolveMultipleInstances<T>()
        {
            var instance1 = ApplicationServices.GetRequiredService<T>();
            var instance2 = ApplicationServices.GetRequiredService<T>();
            return !instance1.Equals(instance2);
        }
    }
}
