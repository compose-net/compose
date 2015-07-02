using Compose.Tests.Fake;
using System;
using System.Threading;
using System.Threading.Tasks;
using TestAttributes;
using Xunit;

namespace Compose.Tests
{
	public class ExecutableTests
	{
		[Unit]
		public void GivenOnExecuteNotInvokedWhenExecuteIsInvokedThenThrowsException()
		{
			var app = new Executable();

			Action act = () => app.Execute();

			Assert.IsType(typeof(InvalidOperationException), Record.Exception(act));
		}

		[Unit]
		public void GivenApplicationServicesIsNullWhenActionTServiceIsRegisteredThenThrowsException()
		{
			var app = new Executable();

			Action act = () => app.OnExecute<string>((service) => { });

			Assert.IsType(typeof(InvalidOperationException), Record.Exception(act));
		}

		[Unit]
		public void WhenExecuteIsInvokedThenActionIsInvoked()
		{
			var executed = false;
			var app = new Executable();
			app.OnExecute(() => { executed = true; });

			app.Execute();

			Assert.True(executed);
		}

		[Unit]
		public void GivenFunctionIsRegisteredWhenExecuteIsInvokedThenFunctionIsInvoked()
		{
			var executed = false;
			var app = new Executable();
			var cts = new CancellationTokenSource();
			app.OnExecute((ct) => { executed = true; return Task.FromResult(false); });

			app.Execute();

			Assert.True(executed);
		}

		[Unit]
		public void GivenBothActionAndFunctionAreRegisteredWhenExecuteIsInvokedThenActionIsInvoked()
		{
			var executed = 0;
			var app = new Executable();

			app.OnExecute((ct) => { executed = 1; return Task.FromResult(false); });
			app.OnExecute(() => { executed = 2; });

			app.Execute();

			Assert.Equal(2, executed);
		}

		[Unit]
		public void GivenOnExecuteNotInvokedWhenExecuteAsyncIsInvokedThenThrowsException()
		{
			var app = new Executable();
			var ct = new CancellationToken();

			Assert.IsType<InvalidOperationException>(Record.ExceptionAsync(async () => await app.ExecuteAsync(ct)).Result);
		}

		[Unit]
		public async void WhenExecuteAsyncIsInvokedThenActionIsInvoked()
		{
			var executed = false;
			var app = new Executable();
			var cts = new CancellationTokenSource();
			app.OnExecute(() => { executed = true; });

			await app.ExecuteAsync(cts.Token);

			Assert.True(executed);
		}

		[Unit]
		public async void GivenFunctionIsRegisteredWhenExecuteAsyncIsInvokedThenFunctionIsInvoked()
		{
			var executed = false;
			var app = new Executable();
			var cts = new CancellationTokenSource();
			app.OnExecute((ct) => { executed = true; return Task.FromResult(false); });

			await app.ExecuteAsync(cts.Token);

			Assert.True(executed);
		}

		[Unit]
		public async void GivenBothActionAndFunctionAreRegisteredWhenExecuteAsyncIsInvokedThenFuncIsInvoked()
		{
			var executed = 0;
			var app = new Executable();
			var cts = new CancellationTokenSource();

			app.OnExecute((ct) => { executed = 1; return Task.FromResult(false); });
			app.OnExecute(() => { executed = 2; });

			await app.ExecuteAsync(cts.Token);

			Assert.Equal(1, executed);
		}
	}

	public class ExecutableTResult
	{
		[Unit]
		public void GivenOnExecuteNotInvokedWhenExecuteIsInvokedThenThrowsException()
		{
			var app = new FakeExecutable();

			Action act = () => app.Execute();

			Assert.IsType(typeof(InvalidOperationException), Record.Exception(act));
		}

		[Unit]
		public void GivenApplicationServicesIsNullWhenActionTServiceIsRegisteredThenThrowsException()
		{
			var app = new FakeExecutable();

			Action act = () => app.OnExecute<string>((service) => { return false; });

			Assert.IsType(typeof(InvalidOperationException), Record.Exception(act));
		}

		[Unit]
		public void WhenExecuteIsInvokedThenActionIsInvoked()
		{
			var executed = false;
			var app = new FakeExecutable();
			app.OnExecute(() => { return true; });

			executed = app.Execute();

			Assert.True(executed);
		}

		[Unit]
		public void GivenFunctionIsRegisteredWhenExecuteIsInvokedThenFunctionIsInvoked()
		{
			var executed = false;
			var app = new FakeExecutable();
			var cts = new CancellationTokenSource();
			app.OnExecute((ct) => { executed = true; return Task.FromResult(false); });

			app.Execute();

			Assert.True(executed);
		}

		[Unit]
		public void GivenBothActionAndFunctionAreRegisteredWhenExecuteIsInvokedThenActionIsInvoked()
		{
			var executed = 0;
			var app = new FakeExecutable();

			app.OnExecute((ct) => { executed = 1; return Task.FromResult(false); });
			app.OnExecute(() => { executed = 2; return false; });

			app.Execute();

			Assert.Equal(2, executed);
		}

		[Unit]
		public void GivenOnExecuteNotInvokedWhenExecuteAsyncIsInvokedThenThrowsException()
		{
			var app = new FakeExecutable();
			var ct = new CancellationToken();

			Assert.IsType<InvalidOperationException>(Record.ExceptionAsync(async () => await app.ExecuteAsync(ct)).Result);
		}

		[Unit]
		public async void WhenExecuteAsyncIsInvokedThenActionIsInvoked()
		{
			var executed = false;
			var app = new FakeExecutable();
			var cts = new CancellationTokenSource();
			app.OnExecute(() => { return true; });

			executed = await app.ExecuteAsync(cts.Token);

			Assert.True(executed);
		}

		[Unit]
		public async void GivenFunctionIsRegisteredWhenExecuteAsyncIsInvokedThenFunctionIsInvoked()
		{
			var executed = false;
			var app = new FakeExecutable();
			var cts = new CancellationTokenSource();
			app.OnExecute((ct) => { executed = true; return Task.FromResult(false); });

			await app.ExecuteAsync(cts.Token);

			Assert.True(executed);
		}

		[Unit]
		public async void GivenBothActionAndFunctionAreRegisteredWhenExecuteAsyncIsInvokedThenFuncIsInvoked()
		{
			var executed = 0;
			var app = new FakeExecutable();
			var cts = new CancellationTokenSource();

			app.OnExecute((ct) => { executed = 1; return Task.FromResult(false); });
			app.OnExecute(() => { executed = 2; return false; });

			await app.ExecuteAsync(cts.Token);

			Assert.Equal(1, executed);
		}
	}

	public class ExecutableTContext
	{
		[Unit]
		public void GivenOnExecuteNotInvokedWhenExecuteIsInvokedThenThrowsException()
		{
			var app = new FakeContextExecutable();

			Action act = () => app.Execute(false);

			Assert.IsType(typeof(InvalidOperationException), Record.Exception(act));
		}

		[Unit]
		public void GivenApplicationServicesIsNullWhenActionTServiceIsRegisteredThenThrowsException()
		{
			var app = new FakeContextExecutable();

			Action act = () => app.OnExecute<string>((service, boo) => { return false; });

			Assert.IsType(typeof(InvalidOperationException), Record.Exception(act));
		}

		[Unit]
		public void WhenExecuteIsInvokedThenActionIsInvoked()
		{
			var executed = false;
			var app = new FakeContextExecutable();
			app.OnExecute((boo) => { return true; });

			executed = app.Execute(false);

			Assert.True(executed);
		}

		[Unit]
		public void GivenFunctionIsRegisteredWhenExecuteIsInvokedThenFunctionIsInvoked()
		{
			var executed = false;
			var app = new FakeContextExecutable();
			var cts = new CancellationTokenSource();
			app.OnExecute((boo, ct) => { executed = true; return Task.FromResult(false); });

			app.Execute(false);

			Assert.True(executed);
		}

		[Unit]
		public void GivenBothActionAndFunctionAreRegisteredWhenExecuteIsInvokedThenActionIsInvoked()
		{
			var executed = 0;
			var app = new FakeContextExecutable();

			app.OnExecute((boo, ct) => { executed = 1; return Task.FromResult(false); });
			app.OnExecute((boo) => { executed = 2; return false; });

			app.Execute(false);

			Assert.Equal(2, executed);
		}

		[Unit]
		public void GivenOnExecuteNotInvokedWhenExecuteAsyncIsInvokedThenThrowsException()
		{
			var app = new FakeContextExecutable();
			var ct = new CancellationToken();

			Assert.IsType<InvalidOperationException>(Record.ExceptionAsync(async () => await app.ExecuteAsync(false, ct)).Result);
		}

		[Unit]
		public async void WhenExecuteAsyncIsInvokedThenActionIsInvoked()
		{
			var executed = false;
			var app = new FakeContextExecutable();
			var cts = new CancellationTokenSource();
			app.OnExecute((boo) => { return true; });

			executed = await app.ExecuteAsync(false, cts.Token);

			Assert.True(executed);
		}

		[Unit]
		public async void GivenFunctionIsRegisteredWhenExecuteAsyncIsInvokedThenFunctionIsInvoked()
		{
			var executed = false;
			var app = new FakeContextExecutable();
			var cts = new CancellationTokenSource();
			app.OnExecute((boo, ct) => { executed = true; return Task.FromResult(false); });

			await app.ExecuteAsync(false, cts.Token);

			Assert.True(executed);
		}

		[Unit]
		public async void GivenBothActionAndFunctionAreRegisteredWhenExecuteAsyncIsInvokedThenFuncIsInvoked()
		{
			var executed = 0;
			var app = new FakeContextExecutable();
			var cts = new CancellationTokenSource();

			app.OnExecute((boo, ct) => { executed = 1; return Task.FromResult(false); });
			app.OnExecute((boo) => { executed = 2; return false; });

			await app.ExecuteAsync(false, cts.Token);

			Assert.Equal(1, executed);
		}
	}
}