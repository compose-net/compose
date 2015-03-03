using System;

namespace Transition
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("Welcome to the Compose sample application!");
			Console.WriteLine("Press any key to start the first demo.");
			Console.ReadKey();
			Console.Clear();

			new Demos.StandardBuildup().Execute();
			new Demos.CodedOverride().Execute();
			new Demos.StandardBuildup().Execute();
		}
	}
}
