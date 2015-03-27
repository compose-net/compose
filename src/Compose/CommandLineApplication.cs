using System.Threading;

namespace Compose
{
	public class CommandLineApplication : Executable<int> { }
	public class CancellableCommandLineApplication : Executable<CancellationToken, int> { }
}
