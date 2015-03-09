using Compose;
using System;
using Transition.Service;

namespace Transition.Demos
{
	internal sealed class ProxyTransition : Executable
	{
		private Func<bool, ConsoleKeyInfo> _readKey = Console.ReadKey;
		private IAsyncResult _result = default(IAsyncResult);
		private ConsoleKeyInfo _keyPress = default(ConsoleKeyInfo);

		private const string UpperMessage = "This is a spam message; press a key to see it in upper case!!";
		private const string LowerMessage = "This is a spam message; press a key to see it in normal case!!";

		private string _message = UpperMessage;

		public ProxyTransition()
		{
			Name = "Proxy Transition" ;
			this.UseServices(services =>
			{
				services.AddSampleService()
					.AsTransitional();
			});
			OnExecute<IHost>(Run);
		}

		private void Run(IHost console)
		{
			console.WriteLine("Press escape to exit...");
			console.WriteLine("Press any other key to switch between Uppercase/Standard write services...");

			_result = _readKey.BeginInvoke(true, null, null);
			while (_keyPress.Key != ConsoleKey.Escape)
			{
				if (_result.AsyncWaitHandle.WaitOne(2000)) _message = Transition();
				console.WriteLine(_message);
			}
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
			this.Transition<IHost, UppercaseHost>();
			return LowerMessage;
		}

		private string TransitionToLowercase()
		{
			this.Transition<IHost, StandardHost>();
			return UpperMessage;
		}
	}
}
