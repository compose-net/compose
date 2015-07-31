namespace Compose.Tests.Fake
{
	internal class Executable : Compose.Executable
	{
		public override void Execute()
		{
			if (Execution == null)
				OnExecute(() => { });
			base.Execute();
		}
	}
}
