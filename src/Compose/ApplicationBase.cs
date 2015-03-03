using Microsoft.Framework.DependencyInjection;

namespace Compose
{
	public class ApplicationBase
	{
		internal ServiceCollection Services { get; } = new ServiceCollection();

		internal RootServiceProvider Provider { get; set; }
	}
}
