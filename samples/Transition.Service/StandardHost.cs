namespace Transition.Service
{
	public class StandardHost : CommonHost
	{
		public override void WriteLine(string message) { System.Console.WriteLine(message); }
	}
}
