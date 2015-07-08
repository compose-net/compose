namespace Compose
{
	public class CommandLineApplication : CommandLineApplication<int> { }
	public class CommandLineApplication<TResult> : Executable<TResult> { }
	public class CommandLineApplication<TContext, TResult> : Executable<TContext, TResult> { }
}
