using Compose.Tests.Fake;
using FluentAssertions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Compose.Tests
{
	public class ExecutableTests
	{
		[Fact]
		public void GivenOnExecuteNotInvokedWhenExecuteIsInvokedThenThrowsException()
		{
			var app = new Executable();

			Action act = () => app.Execute();

			act.ShouldThrow<InvalidOperationException>();
		}

		[Fact]
		public void WhenExecuteIsInvokedThenActionIsInvoked()
		{
			var executed = false;
			var app = new Executable();
			app.OnExecute(() => { executed = true; });

			app.Execute();

			executed.Should().BeTrue();
		}

		[Fact]
		public void GivenFunctionIsRegisteredWhenExecuteIsInvokedThenFunctionIsInvoked()
		{
			var executed = false;
			var app = new Executable();
			var cts = new CancellationTokenSource();
			app.OnExecute((ct) => { executed = true; return Task.FromResult(false); });

			app.Execute();

			executed.Should().BeTrue();
		}

		[Fact]
		public void GivenBothActionAndFunctionAreRegisteredWhenExecuteIsInvokedThenActionIsInvoked()
		{
			var executed = 0;
			var app = new Executable();

			app.OnExecute((ct) => { executed = 1; return Task.FromResult(false); });
			app.OnExecute(() => { executed = 2; });

			app.Execute();

			executed.Should().Be(2);
		}

		[Fact]
		public void GivenOnExecuteNotInvokedWhenExecuteAsyncIsInvokedThenThrowsException()
		{
			var app = new Executable();
			var ct = new CancellationToken();

			Func<Task> act = async () => await app.ExecuteAsync(ct);

			act.ShouldThrow<InvalidOperationException>();
		}

		[Fact]
		public async void WhenExecuteAsyncIsInvokedThenActionIsInvoked()
		{
			var executed = false;
			var app = new Executable();
			var cts = new CancellationTokenSource();
			app.OnExecute(() => { executed = true; });

			await app.ExecuteAsync(cts.Token);

			executed.Should().BeTrue();
		}

		[Fact]
		public async void GivenFunctionIsRegisteredWhenExecuteAsyncIsInvokedThenFunctionIsInvoked()
		{
			var executed = false;
			var app = new Executable();
			var cts = new CancellationTokenSource();
			app.OnExecute((ct) => { executed = true; return Task.FromResult(false); });

			await app.ExecuteAsync(cts.Token);

			executed.Should().BeTrue();
		}

		[Fact]
		public async void GivenBothActionAndFunctionAreRegisteredWhenExecuteAsyncIsInvokedThenFuncIsInvoked()
		{
			var executed = 0;
			var app = new Executable();
			var cts = new CancellationTokenSource();

			app.OnExecute((ct) => { executed = 1; return Task.FromResult(false); });
			app.OnExecute(() => { executed = 2; });

			await app.ExecuteAsync(cts.Token);

			executed.Should().Be(1);
		}

		[Fact]
		public void WhenExecuteTResultIsInvokedThenActionIsInvoked()
		{
			var executed = false;
			var app = new FakeExecutable();
			app.OnExecute(() => { return true; });

			executed = app.Execute();

			executed.Should().BeTrue();
		}

		[Fact]
		public async void WhenExecuteAsyncTResultIsInvokedThenActionIsInvoked()
		{
			var executed = false;
			var app = new FakeExecutable();
			var cts = new CancellationTokenSource();
			app.OnExecute(() => { return true; });

			executed = await app.ExecuteAsync(cts.Token);

			executed.Should().BeTrue();
		}

		[Fact]
		public void WhenExecuteTContextIsInvokedThenActionIsInvoked()
		{
			var executed = false;
			var app = new FakeContextExecutable();
			app.OnExecute((x) => { return x; });

			executed = app.Execute(true);

			executed.Should().BeTrue();
		}

		[Fact]
		public async void WhenExecuteAsyncTContextIsInvokedThenActionIsInvoked()
		{
			var executed = false;
			var app = new FakeContextExecutable();
			var cts = new CancellationTokenSource();
			app.OnExecute((x) => { return x; });

			executed = await app.ExecuteAsync(true, cts.Token);

			executed.Should().BeTrue();
		}
	}
}