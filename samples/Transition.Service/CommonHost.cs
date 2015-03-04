using System;

namespace Transition.Service
{
	public abstract class CommonHost : IHost
	{
		public void Clear() { Console.Clear(); }

		public ConsoleKeyInfo ReadKey() { return Console.ReadKey(); }

		public void WriteLine() { Console.WriteLine(); }

		public abstract void WriteLine(string message);
	}
}
