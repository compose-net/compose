using System;

namespace Compose.Tests.Fake
{
    internal sealed class IndirectlyDisposableImplementation : DisposableService
    {
        public Type ServiceType => GetType();

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
