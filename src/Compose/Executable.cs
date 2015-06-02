using Microsoft.Framework.DependencyInjection;
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

	public abstract class Executable<TResult> : Application
	{
		protected Func<TResult> Execution { get; private set; }
		protected Func<CancellationToken, Task<TResult>> ExecutionAsync { get; private set; }

		public void OnExecute(Func<TResult> invoke)
		{
			Execution = invoke;
		}

		public void OnExecute(Func<CancellationToken, Task<TResult>> asyncInvoke)
		{
			ExecutionAsync = asyncInvoke;
		}

		public void OnExecute<TService>(Func<TService, TResult> invoke) where TService : class
		{
			if (ApplicationServices == null) throw new InvalidOperationException($"{nameof(ApplicationServices)} was not registered; cannot execute action.");

			OnExecute(() => invoke(ApplicationServices.GetRequiredService<TService>()));
		}

		public virtual TResult Execute()
		{
			if (Execution != null)
				return Execution();
			else if (ExecutionAsync != null)
				return ExecutionAsync(CancellationToken.None).Result;
			else
				throw new InvalidOperationException("Cannot execute without invokable action");
		}

		public virtual async Task<TResult> ExecuteAsync(CancellationToken cancellationToken)
		{
			if (ExecutionAsync != null)
				return await ExecutionAsync(cancellationToken);
			else if (Execution != null)
				return await Task.Run(Execution, cancellationToken);
			else
				throw new InvalidOperationException("Cannot execute without invokable action");
		}
	}

	public abstract class Executable<TContext, TResult> : Application
	{
		protected Func<TContext, TResult> Execution { get; private set; }
		protected Func<TContext, CancellationToken, Task<TResult>> ExecutionAsync { get; private set; }

		public void OnExecute(Func<TContext, TResult> invoke)
		{
			Execution = invoke;
		}

		public void OnExecute(Func<TContext, CancellationToken, Task<TResult>> asyncInvoke)
		{
			ExecutionAsync = asyncInvoke;
		}

		public void OnExecute<TService>(Func<TService, TContext, TResult> invoke) where TService : class
		{
			OnExecute(context => invoke(ApplicationServices.GetRequiredService<TService>(), context));
		}

		public virtual TResult Execute(TContext context)
		{
			if (Execution != null)
				return Execution(context);
			else if (ExecutionAsync != null)
				return ExecutionAsync(context, CancellationToken.None).Result;
			else
				throw new InvalidOperationException("Cannot execute without invokable action");
		}

		public virtual async Task<TResult> ExecuteAsync(TContext context, CancellationToken cancellationToken)
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
