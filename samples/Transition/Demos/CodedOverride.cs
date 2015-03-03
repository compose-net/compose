using Compose;
using Microsoft.Framework.DependencyInjection;
using Transition.Service;

namespace Transition.Demos
{
	internal sealed class CodedOverride : Executable
	{
		public CodedOverride()
		{
			Name = "Coded Override";
			this.UseServices(services =>
			{
				services.AddSampleServices();
				services.AddTransient<IHost, UppercaseHost>();
			});
			OnExecute<IHost>(Run);
		}

		private void Run(IHost console)
		{
			console.WriteLine("As will be fairly obvious, the text here is upper case.");
			console.WriteLine("This is because the default IHost implementation has been overriden.");
			console.WriteLine("The same could have been achieved by having a configuration setting matching");
			console.WriteLine($"\"{typeof(IHost).FullName}\"");
			console.WriteLine("and the setting's value containing the fully qualified name of an implementation");
			console.WriteLine();
			console.WriteLine("Press any key to close this application");
			console.ReadKey();
			console.Clear();
		}
	}
}
