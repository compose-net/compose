namespace Transition
{
	internal class UppercaseWriter : IWriter
	{
		public void WriteLine(string message) { System.Console.WriteLine(message.ToUpperInvariant()); }
	}
}
