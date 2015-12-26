using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Compose
{
    public class Executable : Application
    {
        protected Action Execution { get; private set; }
        protected Func<CancellationToken, Task> ExecutionAsync { get; private set; }

        public void OnExecute(Action invoke)
        {
            Execution = invoke;
        }

        public void OnExecute(Func<CancellationToken, Task> asyncInvoke)
        {
            ExecutionAsync = asyncInvoke;
        }

        public void OnExecute<TService>(Action<TService> invoke) where TService : class
        {
            if (ApplicationServices == null) throw new InvalidOperationException($"{nameof(ApplicationServices)} was not registered; cannot execute action.");

            OnExecute(() => invoke(ApplicationServices.GetRequiredService<TService>()));
        }

        public virtual void Execute()
        {
            if (Execution != null)
                Execution();
            else if (ExecutionAsync != null)
                ExecutionAsync(CancellationToken.None).Wait();
            else
                throw new InvalidOperationException("Cannot execute without invokable action");
        }

        public virtual async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            if (ExecutionAsync != null)
                await ExecutionAsync(cancellationToken);
            else if (Execution != null)
                await Task.Run(Execution, cancellationToken);
            else
                throw new InvalidOperationException("Cannot execute without invokable action");
        }
    }

    public abstract class Executable<Result> : Application
    {
        protected Func<Result> Execution { get; private set; }
        protected Func<CancellationToken, Task<Result>> ExecutionAsync { get; private set; }

        public void OnExecute(Func<Result> invoke)
        {
            Execution = invoke;
        }

        public void OnExecute(Func<CancellationToken, Task<Result>> asyncInvoke)
        {
            ExecutionAsync = asyncInvoke;
        }

        public void OnExecute<Service>(Func<Service, Result> invoke) where Service : class
        {
            if (ApplicationServices == null) throw new InvalidOperationException($"{nameof(ApplicationServices)} was not registered; cannot execute action.");

            OnExecute(() => invoke(ApplicationServices.GetRequiredService<Service>()));
        }

        public virtual Result Execute()
        {
            if (Execution != null)
                return Execution();
            else if (ExecutionAsync != null)
                return ExecutionAsync(CancellationToken.None).Result;
            else
                throw new InvalidOperationException("Cannot execute without invokable action");
        }

        public virtual async Task<Result> ExecuteAsync(CancellationToken cancellationToken)
        {
            if (ExecutionAsync != null)
                return await ExecutionAsync(cancellationToken);
            else if (Execution != null)
                return await Task.Run(Execution, cancellationToken);
            else
                throw new InvalidOperationException("Cannot execute without invokable action");
        }

        public abstract class ContextOnly : Application
        {
            protected Action<Result> Execution { get; private set; }
            protected Func<CancellationToken, Result, Task> ExecutionAsync { get; private set; }

            public void OnExecute(Action<Result> invoke)
            {
                Execution = invoke;
            }

            public void OnExecute(Func<CancellationToken, Result, Task> asyncInvoke)
            {
                ExecutionAsync = asyncInvoke;
            }

            public void OnExecute<Service>(Action<Service, Result> invoke)
            {
                if (ApplicationServices == null) throw new InvalidOperationException($"{nameof(ApplicationServices)} was not registered; cannot execute action.");

                OnExecute(context => invoke(ApplicationServices.GetRequiredService<Service>(), context));
            }

            public virtual void Execute(Result context)
            {
                if (Execution != null)
                    Execution(context);
                else if (ExecutionAsync != null)
                    ExecutionAsync(CancellationToken.None, context).Wait();
                else
                    throw new InvalidOperationException("Cannot execute without invokable action");
            }

            public virtual async Task ExecuteAsync(CancellationToken cancellationToken, Result context)
            {
                if (ExecutionAsync != null)
                    await ExecutionAsync(cancellationToken, context);
                else if (Execution != null)
                    await Task.Run(() => Execution(context), cancellationToken);
                else
                    throw new InvalidOperationException("Cannot execute without invokable action");
            }
        }
    }

    public abstract class Executable<Context, Result> : Application
    {
        protected Func<Context, Result> Execution { get; private set; }
        protected Func<Context, CancellationToken, Task<Result>> ExecutionAsync { get; private set; }

        public void OnExecute(Func<Context, Result> invoke)
        {
            Execution = invoke;
        }

        public void OnExecute(Func<Context, CancellationToken, Task<Result>> asyncInvoke)
        {
            ExecutionAsync = asyncInvoke;
        }

        public void OnExecute<Service>(Func<Service, Context, Result> invoke) where Service : class
        {
            if (ApplicationServices == null) throw new InvalidOperationException($"{nameof(ApplicationServices)} was not registered; cannot execute action.");

            OnExecute(context => invoke(ApplicationServices.GetRequiredService<Service>(), context));
        }

        public virtual Result Execute(Context context)
        {
            if (Execution != null)
                return Execution(context);
            else if (ExecutionAsync != null)
                return ExecutionAsync(context, CancellationToken.None).Result;
            else
                throw new InvalidOperationException("Cannot execute without invokable action");
        }

        public virtual async Task<Result> ExecuteAsync(Context context, CancellationToken cancellationToken)
        {
            if (ExecutionAsync != null)
                return await ExecutionAsync(context, cancellationToken);
            else if (Execution != null)
                return await Task.Run(() => Execution(context), cancellationToken);
            else
                throw new InvalidOperationException("Cannot execute without invokable action");
        }
    }
}
