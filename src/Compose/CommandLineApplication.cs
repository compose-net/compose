using System.Threading;

namespace Compose
{
	public class CommandLineApplication : CommandLineApplication<int> { }
	public class CommandLineApplication<TResult> : Executable<TResult> { }
	public class CancellableCommandLineApplication : Executable<CancellationToken, int> { }
}
