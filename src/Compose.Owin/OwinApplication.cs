using Microsoft.Owin;
using Owin;
using System;
using System.Threading.Tasks;

namespace Compose
{
    public class OwinApplication : Application
    {
        private Action _execution { get; set; }

        public void OnExecute(IAppBuilder appbuilder, Func<IOwinContext, Task> invoke)
        {
            _execution = () => appbuilder.Run(invoke);
        }

		public void OnExecute<TService>(IAppBuilder appbuilder, Func<TService, IOwinContext, Task> invoke) where TService : class
		{
			OnExecute(appbuilder, (context) => invoke(GetRequiredService<TService>(), context));
		}

        public void Execute()
        {
            _execution();
        }
    }
}
