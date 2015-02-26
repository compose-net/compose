using Compose;
using System;

namespace Transition
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var app = new CommandLineApplication();
			app.Name = "Composition Transitions";

			app.UseServices(services =>
			{
				services.AddTransitional<IWriter, TransitionalStandardWriter>();
			});

			Console.WriteLine("Welcome to the transitional example application!");
			Console.WriteLine("This application demonstrates how application composition can be modified seamlessly at runtime.");
			Console.WriteLine("");

			app.OnExecute<IWriter>(new DirectTransitionExample(app).Run);
			app.Execute();
		}
	}
}
