using System;

namespace Compose.Tests.Fake
{
    internal sealed class DirectlyDisposableImplementation : Service, IDisposable
    {
        public Type ServiceType => GetType();

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
