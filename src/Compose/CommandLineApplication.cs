using System.Threading;

namespace Compose
{
	public class CommandLineApplication : CommandLineApplciation<int> { }
	public class CommandLineApplciation<TResult> : Executable<TResult> { }
	public class CancellableCommandLineApplication : Executable<CancellationToken, int> { }
}
