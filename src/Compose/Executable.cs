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

	public abstract class Executable<T> : Application
	{
		protected Func<T> Execution { get; private set; }
		protected Func<CancellationToken, Task<T>> ExecutionAsync { get; private set; }

		public void OnExecute(Func<T> invoke)
		{
			Execution = invoke;
		}

		public void OnExecute(Func<CancellationToken, Task<T>> asyncInvoke)
		{
			ExecutionAsync = asyncInvoke;
		}

		public void OnExecute<TService>(Func<TService, T> invoke) where TService : class
		{
			if (ApplicationServices == null) throw new InvalidOperationException($"{nameof(ApplicationServices)} was not registered; cannot execute action.");

			OnExecute(() => invoke(ApplicationServices.GetRequiredService<TService>()));
		}

		public virtual T Execute()
		{
			if (Execution != null)
				return Execution();
			else if (ExecutionAsync != null)
				return ExecutionAsync(CancellationToken.None).Result;
			else
				throw new InvalidOperationException("Cannot execute without invokable action");
		}

		public virtual async Task<T> ExecuteAsync(CancellationToken cancellationToken)
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
			protected Action<T> Execution { get; private set; }
			protected Func<CancellationToken, T, Task> ExecutionAsync { get; private set; }

			public void OnExecute(Action<T> invoke)
			{
				Execution = invoke;
			}

			public void OnExecute(Func<CancellationToken, Task> asyncInvoke)
			{
				ExecutionAsync = asyncInvoke;
			}

			public void OnExecute<TService>(Action<TService, T> invoke)
			{
				if (ApplicationServices == null) throw new InvalidOperationException($"{nameof(ApplicationServices)} was not registered; cannot execute action.");

				OnExecute(context => invoke(ApplicationServices.GetRequiredService<TService>(), context));
			}

			public virtual void Execute(T context)
			{
				if (Execution != null)
					Execution(context);
				else if (ExecutionAsync != null)
					return ExecutionAsync(CancellationToken.None).Result;
				else
					throw new InvalidOperationException("Cannot execute without invokable action");
			}

			public virtual async Task ExecuteAsync(CancellationToken cancellationToken, T context)
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
			if (ApplicationServices == null) throw new InvalidOperationException($"{nameof(ApplicationServices)} was not registered; cannot execute action.");

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
