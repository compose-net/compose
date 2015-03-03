using Compose;
using Transition.Service;

namespace Transition.Demos
{
	internal sealed class StandardBuildup : Executable
	{
		public StandardBuildup()
		{
			Name = "Standard Composition";
			this.UseServices(services =>
			{
				services.AddSampleServices();
			});
			OnExecute<IHost>(Run);
		}

		private void Run(IHost console)
		{
			console.WriteLine("This application has been built up using the default services defined by the service provider.");
			console.WriteLine("With no knowledge of how the IHost has been implemented, we can safely use IHost, not only as");
			console.WriteLine("the application entry dependency, but anywhere within our composition graph!");
			console.WriteLine();
			console.WriteLine("Press any key to close this application");
			console.ReadKey();
			console.Clear();
		}
	}
}
