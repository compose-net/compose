using Compose;
using System;

namespace Transition
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var app = new CommandLineApplication();
			app.Name = "Direct Transitions";

			app.UseServices(services =>
			{
				services.AddTransitional<IWriter, TransitionalStandardWriter>();
			});

			Console.WriteLine("Welcome to the transitional example application!");
			Console.WriteLine("This application demonstrates how application composition can be modified seamlessly at runtime.");
			Console.WriteLine("");

			Console.WriteLine("The following is an example of direct transitions...");
			app.OnExecute<IWriter>(new DirectTransitionExample(app).Run);
			app.Execute();

			app = new CommandLineApplication();
			app.Name = "Factory Transitions";

			app.UseServices(services =>
			{
				services.AddTransitionalFactory<IWriter, StandardWriter>();
			});

			Console.WriteLine("The following is an example of factory transitions...");
			app.OnExecute<IFactory<IWriter>>(new FactoryTransitionExample(app).Run);
			app.Execute();
		}
	}
}
