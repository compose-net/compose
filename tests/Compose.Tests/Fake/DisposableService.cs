using System;

namespace Compose.Tests.Fake
{
    internal interface DisposableService : Service, IDisposable
    {
    }
}
