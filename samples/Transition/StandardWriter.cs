namespace Transition
{
	internal class StandardWriter : IWriter
	{
		public void WriteLine(string message) { System.Console.WriteLine(message); }
	}
}
