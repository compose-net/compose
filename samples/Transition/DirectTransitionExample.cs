using Compose;
using System;

namespace Transition
{
	internal class DirectTransitionExample
	{
		private readonly Application _app;

		public DirectTransitionExample(Application app) { _app = app; }

		private Func<bool, ConsoleKeyInfo> _readKey = Console.ReadKey;
		private IAsyncResult _result = default(IAsyncResult);
		private ConsoleKeyInfo _keyPress = default(ConsoleKeyInfo);

		private const string UpperMessage = "This is a spam message; press a key to see it in upper case!!";
		private const string LowerMessage = "This is a spam message; press a key to see it in normal case!!";

		private string _message = UpperMessage;

		public int Run(IWriter service)
		{
			Console.WriteLine("Press escape to exit...");
			Console.WriteLine("Press any other key to change switch to the Uppercase write services...");

			_result = _readKey.BeginInvoke(true, null, null);
			while (_keyPress.Key != ConsoleKey.Escape)
			{
				if (_result.AsyncWaitHandle.WaitOne(2000)) _message = Transition();
				service.WriteLine(_message);
			}
			return 0;
		}

		private string Transition()
		{
			_keyPress = _readKey.EndInvoke(_result);
			if (_keyPress.Key == ConsoleKey.Escape) return "Exiting...";

			_result = _readKey.BeginInvoke(true, null, null);
			if (_message == UpperMessage) return TransitionToUppercase();
			return TransitionToLowercase();
		}

		private string TransitionToUppercase()
		{
			_app.Transition<IWriter, UppercaseWriter>();
			return LowerMessage;
		}

		private string TransitionToLowercase()
		{
			_app.Transition<IWriter, StandardWriter>();
			return UpperMessage;
		}
	}
}
