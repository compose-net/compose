using Compose;
using System;

namespace Transition
{
	public class Program
	{
		private static Application _app;

		public static void Main(string[] args)
		{
			var app = new CommandLineApplication();
			_app = app;
			app.Name = "Composition Transitions";

			app.UseServices(services =>
			{
				services.AddTransitional<IWriter, TransitionalStandardWriter>();
			});

			app.OnExecute<IWriter>(Run);
			app.Execute();
		}

		private static int Run(IWriter console)
		{
			try
			{
				console.WriteLine("Welcome to the transitional example application!");
				console.WriteLine("This application demonstrates how application composition can be modified seamlessly at runtime");
				console.WriteLine("");

				console.WriteLine("Press escape to exit...");
				console.WriteLine("Press any other key to change switch to the Uppercase write services...");
				Func<bool, ConsoleKeyInfo> readKey = Console.ReadKey;
				var result = readKey.BeginInvoke(true, null, null);
				ConsoleKeyInfo keyPress = default(ConsoleKeyInfo);
				while (keyPress.Key != ConsoleKey.Escape)
				{
					if (result.AsyncWaitHandle.WaitOne(2000)) Transition(readKey, ref result, ref keyPress);
					console.WriteLine("This is a spam message; press a key to see it in upper case!!");
				}
				return 0;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				Console.ReadKey();
				return 1;
			}
		}

		private static void Transition(Func<bool, ConsoleKeyInfo> readKey, ref IAsyncResult result, ref ConsoleKeyInfo keyPress)
		{
			keyPress = readKey.EndInvoke(result);
			if (keyPress.Key == ConsoleKey.Escape) return;
            _app.Transition<IWriter, UppercaseWriter>();
			result = readKey.BeginInvoke(true, null, null);
		}

		private interface IWriter
		{
			void WriteLine(string message);
		}

		private class TransitionalStandardWriter : DirectTransition<IWriter, StandardWriter>, IWriter
		{
			public TransitionalStandardWriter(StandardWriter service) : base(service) { }

			public void WriteLine(string message)
			{
				Service.WriteLine(message);
			}
		}

		private class StandardWriter : IWriter
		{
			public void WriteLine(string message) { Console.WriteLine(message); }
		}

		private class UppercaseWriter : IWriter
		{
			public void WriteLine(string message) { Console.WriteLine(message.ToUpperInvariant()); }
		}
	}
}
