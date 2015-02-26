namespace Compose
{
	public class CommandLineApplication : Executable<int>
	{
		public int Execute() { return Execution(); }
	}
}
