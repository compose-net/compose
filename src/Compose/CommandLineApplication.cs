namespace Compose
{
	public class CommandLineApplication : CommandLineApplication<int> { }
	public class CommandLineApplication<T> : Executable<T> { }
	public class CommandLineApplication<TContext, TResult> : Executable<TContext, TResult> { }
}
