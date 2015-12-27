using FluentAssertions;
using System;
using System.Threading;
using System.Threading.Tasks;
using TestAttributes;

namespace Compose.Tests
{
    public class ExecutableTests
    {
        [Unit]
        public void GivenAnExecutableWhenExecuteIsInvokedAndOnExecuteIsNotInvokedThenThrowsException()
        {
            var app = new Executable();

            Action act = () => app.Execute();

            act.ShouldThrow<InvalidOperationException>();
        }

        [Unit]
        public void GivenAnExecutableWhenExecuteIsInvokedThenActionIsInvoked()
        {
            var executed = false;
            var app = new Executable();
            app.OnExecute(() => { executed = true; });

            app.Execute();

            executed.Should().BeTrue();
        }

        [Unit]
        public void GivenAnExecutableWhenExecuteIsInvokedThenRegisteredFunctionIsInvoked()
        {
            var executed = false;
            var app = new Executable();
            var cts = new CancellationTokenSource();
            app.OnExecute((ct) => { executed = true; return Task.FromResult(false); });

            app.Execute();

            executed.Should().BeTrue();
        }

        [Unit]
        public void GivenAnExecutableWhenExecuteIsInvokedAndBothActionAndFunctionAreRegisteredThenActionIsInvoked()
        {
            var executed = 0;
            var app = new Executable();

            app.OnExecute((ct) => { executed = 1; return Task.FromResult(false); });
            app.OnExecute(() => { executed = 2; });

            app.Execute();

            executed.Should().Be(2);
        }

        [Unit]
        public void GivenAnExecutableWhenExecuteAsyncIsInvokedAndOnExecuteIsNotInvokedThenThrowsException()
        {
            var app = new Executable();
            var ct = new CancellationToken();

            Action act = () => app.ExecuteAsync(ct).Wait();

            act.ShouldThrow<InvalidOperationException>();
        }

        [Unit]
        public async void GivenAnExecutableWhenExecuteAsyncIsInvokedThenActionIsInvoked()
        {
            var executed = false;
            var app = new Executable();
            var cts = new CancellationTokenSource();
            app.OnExecute(() => { executed = true; });

            await app.ExecuteAsync(cts.Token);

            executed.Should().BeTrue();
        }

        [Unit]
        public async void GivenAnExecutableWhenExecuteAsyncIsInvokedThenRegisteredFunctionIsInvoked()
        {
            var executed = false;
            var app = new Executable();
            var cts = new CancellationTokenSource();
            app.OnExecute((ct) => { executed = true; return Task.FromResult(false); });

            await app.ExecuteAsync(cts.Token);

            executed.Should().BeTrue();
        }

        [Unit]
        public async void GivenAnExecutableWhenExecuteAsyncIsInvokedAndBothActionAndFunctionAreRegisteredThenFunctionIsInvoked()
        {
            var executed = 0;
            var app = new Executable();
            var cts = new CancellationTokenSource();

            app.OnExecute((ct) => { executed = 1; return Task.FromResult(false); });
            app.OnExecute(() => { executed = 2; });

            await app.ExecuteAsync(cts.Token);

            executed.Should().Be(1);
        }

        [Unit]
        public void GivenAnExecutableOfResultWhenExecuteIsInvokedAndOnExecuteIsNotInvokedThenThrowsException()
        {
            var app = new Fake.FakeExecutable();

            Action act = () => app.Execute();

            act.ShouldThrow<InvalidOperationException>();
        }

        [Unit]
        public void GivenAnExecutableOfResultWhenExecuteIsInvokedThenActionIsInvoked()
        {
            var executed = false;
            var app = new Fake.FakeExecutable();
            app.OnExecute(() => { executed = true; return true; });

            app.Execute();

            executed.Should().BeTrue();
        }

        [Unit]
        public void GivenAnExecutableOfResultWhenExecuteIsInvokedThenRegisteredFunctionIsInvoked()
        {
            var executed = false;
            var app = new Fake.FakeExecutable();
            var cts = new CancellationTokenSource();
            app.OnExecute((ct) => { executed = true; return Task.FromResult(false); });

            app.Execute();

            executed.Should().BeTrue();
        }

        [Unit]
        public void GivenAnExecutableOfResultWhenExecuteIsInvokedAndBothActionAndFunctionAreRegisteredThenActionIsInvoked()
        {
            var executed = 0;
            var app = new Fake.FakeExecutable();

            app.OnExecute((ct) => { executed = 1; return Task.FromResult(false); });
            app.OnExecute(() => { executed = 2; return true; });

            app.Execute();

            executed.Should().Be(2);
        }

        [Unit]
        public void GivenAnExecutableOfResultWhenExecuteAsyncIsInvokedAndOnExecuteIsNotInvokedThenThrowsException()
        {
            var app = new Fake.FakeExecutable();
            var ct = new CancellationToken();

            Action act = () => app.ExecuteAsync(ct).Wait();

            act.ShouldThrow<InvalidOperationException>();
        }

        [Unit]
        public async void GivenAnExecutableOfResultWhenExecuteAsyncIsInvokedThenActionIsInvoked()
        {
            var executed = false;
            var app = new Fake.FakeExecutable();
            var cts = new CancellationTokenSource();
            app.OnExecute(() => { executed = true; return true; });

            await app.ExecuteAsync(cts.Token);

            executed.Should().BeTrue();
        }

        [Unit]
        public async void GivenAnExecutableOfResultWhenExecuteAsyncIsInvokedThenRegisteredFunctionIsInvoked()
        {
            var executed = false;
            var app = new Fake.FakeExecutable();
            var cts = new CancellationTokenSource();
            app.OnExecute((ct) => { executed = true; return Task.FromResult(false); });

            await app.ExecuteAsync(cts.Token);

            executed.Should().BeTrue();
        }

        [Unit]
        public async void GivenAnExecutableOfResultWhenExecuteAsyncIsInvokedAndBothActionAndFunctionAreRegisteredThenFunctionIsInvoked()
        {
            var executed = 0;
            var app = new Fake.FakeExecutable();
            var cts = new CancellationTokenSource();

            app.OnExecute((ct) => { executed = 1; return Task.FromResult(false); });
            app.OnExecute(() => { executed = 2; return true; });

            await app.ExecuteAsync(cts.Token);

            executed.Should().Be(1);
        }

        [Unit]
        public void GivenAnExecutableOfContextAndResultWhenExecuteIsInvokedAndOnExecuteIsNotInvokedThenThrowsException()
        {
            var app = new Fake.FakeContextExecutable();

            Action act = () => app.Execute(true);

            act.ShouldThrow<InvalidOperationException>();
        }

        [Unit]
        public void GivenAnExecutableOfContextAndResultWhenExecuteIsInvokedThenActionIsInvoked()
        {
            var executed = false;
            var app = new Fake.FakeContextExecutable();
            app.OnExecute((b) => { executed = true; return true; });

            app.Execute(true);

            executed.Should().BeTrue();
        }

        [Unit]
        public void GivenAnExecutableOfContextAndResultWhenExecuteIsInvokedThenRegisteredFunctionIsInvoked()
        {
            var executed = false;
            var app = new Fake.FakeContextExecutable();
            var cts = new CancellationTokenSource();
            app.OnExecute((ct, b) => { executed = true; return Task.FromResult(false); });

            app.Execute(true);

            executed.Should().BeTrue();
        }

        [Unit]
        public void GivenAnExecutableOfContextAndResultWhenExecuteIsInvokedAndBothActionAndFunctionAreRegisteredThenActionIsInvoked()
        {
            var executed = 0;
            var app = new Fake.FakeContextExecutable();

            app.OnExecute((ct, b) => { executed = 1; return Task.FromResult(false); });
            app.OnExecute((b) => { executed = 2; return true; });

            app.Execute(true);

            executed.Should().Be(2);
        }

        [Unit]
        public void GivenAnExecutableOfContextAndResultWhenExecuteAsyncIsInvokedAndOnExecuteIsNotInvokedThenThrowsException()
        {
            var app = new Fake.FakeContextExecutable();
            var ct = new CancellationToken();

            Action act = () => app.ExecuteAsync(true, ct).Wait();

            act.ShouldThrow<InvalidOperationException>();
        }

        [Unit]
        public async void GivenAnExecutableOfContextAndResultWhenExecuteAsyncIsInvokedThenActionIsInvoked()
        {
            var executed = false;
            var app = new Fake.FakeContextExecutable();
            var cts = new CancellationTokenSource();
            app.OnExecute(b => { executed = true; return true; });

            await app.ExecuteAsync(true, cts.Token);

            executed.Should().BeTrue();
        }

        [Unit]
        public async void GivenAnExecutableOfContextAndResultWhenExecuteAsyncIsInvokedThenRegisteredFunctionIsInvoked()
        {
            var executed = false;
            var app = new Fake.FakeContextExecutable();
            var cts = new CancellationTokenSource();
            app.OnExecute((ct, b) => { executed = true; return Task.FromResult(false); });

            await app.ExecuteAsync(true, cts.Token);

            executed.Should().BeTrue();
        }

        [Unit]
        public async void GivenAnExecutableOfContextAndResultWhenExecuteAsyncIsInvokedAndBothActionAndFunctionAreRegisteredThenFunctionIsInvoked()
        {
            var executed = 0;
            var app = new Fake.FakeContextExecutable();
            var cts = new CancellationTokenSource();

            app.OnExecute((ct, b) => { executed = 1; return Task.FromResult(false); });
            app.OnExecute((b) => { executed = 2; return true; });

            await app.ExecuteAsync(true, cts.Token);

            executed.Should().Be(1);
        }
    }
}