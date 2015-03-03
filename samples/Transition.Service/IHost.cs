using System;

namespace Transition.Service
{
	public interface IHost
	{
		void WriteLine();
		void WriteLine(string message);
		ConsoleKeyInfo ReadKey();
		void Clear();
	}
}
