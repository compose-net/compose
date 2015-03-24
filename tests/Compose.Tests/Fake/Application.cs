namespace Compose.Tests.Fake
{
	internal class Application : Executable
	{
		public override void Execute()
		{
			if (Execution == null)
				OnExecute(() => { });
			base.Execute();
		}
	}
}
