namespace Transition
{
	internal class UppercaseHost : Service.CommonHost
	{
		public override void WriteLine(string message) { System.Console.WriteLine(message.ToUpperInvariant()); }
	}
}
