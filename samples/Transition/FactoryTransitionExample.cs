using Compose;
using System;

namespace Transition
{
	internal sealed class FactoryTransitionExample
	{
		private readonly Application _app;

		public FactoryTransitionExample(Application app) { _app = app; }

		private Func<bool, ConsoleKeyInfo> _readKey = Console.ReadKey;
		private IAsyncResult _result = default(IAsyncResult);
		private ConsoleKeyInfo _keyPress = default(ConsoleKeyInfo);

		private static string[] Messages = new[] { "This is the first message", "This is the second message", "This is the third message" };
		private bool IsUpper = false;

		public int Run(IFactory<IWriter> factory)
		{
			Console.WriteLine("Press escape to exit...");
			Console.WriteLine("Press any other key to switch between Uppercase/Standard write services...");

			_result = _readKey.BeginInvoke(true, null, null);
			while (_keyPress.Key != ConsoleKey.Escape)
			{
				var service = factory.GetService();
				for (var i = 0; i < Messages.Length; i++)
					if (_result.AsyncWaitHandle.WaitOne(500)) service.WriteLine(Transition(i));
					else service.WriteLine(Messages[i]);
			}

			return 0;
		}

		private string Transition(int i)
		{
			_keyPress = _readKey.EndInvoke(_result);
			if (_keyPress.Key == ConsoleKey.Escape) return "Exiting...";

			_result = _readKey.BeginInvoke(true, null, null);
			if (IsUpper) return TransitionToLowercase(i);
			return TransitionToUppercase(i);
		}

		private string TransitionToUppercase(int i)
		{
			_app.Transition<IWriter, UppercaseWriter>();
			IsUpper = true;
			return Messages[i];
		}

		private string TransitionToLowercase(int i)
		{
			_app.Transition<IWriter, StandardWriter>();
			IsUpper = false;
			return Messages[i];
		}
	}
}
