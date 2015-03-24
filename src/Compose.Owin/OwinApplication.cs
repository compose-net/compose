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

        public void Execute()
        {
            _execution();
        }
    }
}
