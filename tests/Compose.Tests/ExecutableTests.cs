using FluentAssertions;
using System;
using System.Threading;
using System.Threading.Tasks;
using TestAttributes;
using Xunit;

namespace Compose.Tests
{
    public class ExecutableTests
    {
        public class GivenAnExecutable
        {
            public class WhenExecuteIsInvoked
            {
                [Unit]
                public void WithOnExecuteNotInvokedThenThrowsException()
                {
                    var app = new Executable();

                    Action act = () => app.Execute();

                    act.ShouldThrow<InvalidOperationException>();
                }

                [Unit]
                public void ThenActionIsInvoked()
                {
                    var executed = false;
                    var app = new Executable();
                    app.OnExecute(() => { executed = true; });

                    app.Execute();

                    executed.Should().BeTrue();
                }

                [Unit]
                public void ThenRegisteredFunctionIsInvoked()
                {
                    var executed = false;
                    var app = new Executable();
                    var cts = new CancellationTokenSource();
                    app.OnExecute((ct) => { executed = true; return Task.FromResult(false); });

                    app.Execute();

                    executed.Should().BeTrue();
                }

                [Unit]
                public void WithBothActionAndFunctionRegistrationsThenActionIsInvoked()
                {
                    var executed = 0;
                    var app = new Executable();

                    app.OnExecute((ct) => { executed = 1; return Task.FromResult(false); });
                    app.OnExecute(() => { executed = 2; });

                    app.Execute();

                    executed.Should().Be(2);
                }
            }

            public class WhenExecuteAsyncIsInvoked
            {
                [Unit]
                public void WithOnExecuteNotInvokedThenThrowsException()
                {
                    var app = new Executable();
                    var ct = new CancellationToken();

                    Action act = async () => await app.ExecuteAsync(ct);

                    act.ShouldThrow<InvalidOperationException>();
                }

                [Unit]
                public async void ThenActionIsInvoked()
                {
                    var executed = false;
                    var app = new Executable();
                    var cts = new CancellationTokenSource();
                    app.OnExecute(() => { executed = true; });

                    await app.ExecuteAsync(cts.Token);

                    executed.Should().BeTrue();
                }

                [Unit]
                public async void ThenRegisteredFunctionIsInvoked()
                {
                    var executed = false;
                    var app = new Executable();
                    var cts = new CancellationTokenSource();
                    app.OnExecute((ct) => { executed = true; return Task.FromResult(false); });

                    await app.ExecuteAsync(cts.Token);

                    executed.Should().BeTrue();
                }

                [Unit]
                public async void WithBothActionAndFunctionRegistrationsThenActionIsInvoked()
                {
                    var executed = 0;
                    var app = new Executable();
                    var cts = new CancellationTokenSource();

                    app.OnExecute((ct) => { executed = 1; return Task.FromResult(false); });
                    app.OnExecute(() => { executed = 2; });

                    await app.ExecuteAsync(cts.Token);

                    executed.Should().Be(2);
                }
            }

            public class WhenOnExecuteIsInvoked
            {
                [Unit]
                public void WithNoServicesRegisteredThenThrowsException()
                {
                    var app = new Executable();

                    Action act = () => app.OnExecute<string>((service) => { });

                    act.ShouldThrow<InvalidOperationException>();
                }
            }
        }

        // TODO :: Restructure tests below. 
        // TODO :: Fix failing tests

        public class GivenAnExecutableOfTResult
        {
            [Unit]
            public void GivenOnExecuteNotInvokedWhenExecuteIsInvokedThenThrowsException()
            {
                var app = new Fake.FakeExecutable();

                Action act = () => app.Execute();

                Assert.IsType(typeof(InvalidOperationException), Record.Exception(act));
            }

            [Unit]
            public void GivenApplicationServicesIsNullWhenActionTServiceIsRegisteredThenThrowsException()
            {
                var app = new Fake.FakeExecutable();

                Action act = () => app.OnExecute<string>((service) => { return false; });

                Assert.IsType(typeof(InvalidOperationException), Record.Exception(act));
            }

            [Unit]
            public void WhenExecuteIsInvokedThenActionIsInvoked()
            {
                var executed = false;
                var app = new Fake.FakeExecutable();
                app.OnExecute(() => { return true; });

                executed = app.Execute();

                Assert.True(executed);
            }

            [Unit]
            public void GivenFunctionIsRegisteredWhenExecuteIsInvokedThenFunctionIsInvoked()
            {
                var executed = false;
                var app = new Fake.FakeExecutable();
                var cts = new CancellationTokenSource();
                app.OnExecute((ct) => { executed = true; return Task.FromResult(false); });

                app.Execute();

                Assert.True(executed);
            }

            [Unit]
            public void GivenBothActionAndFunctionAreRegisteredWhenExecuteIsInvokedThenActionIsInvoked()
            {
                var executed = 0;
                var app = new Fake.FakeExecutable();

                app.OnExecute((ct) => { executed = 1; return Task.FromResult(false); });
                app.OnExecute(() => { executed = 2; return false; });

                app.Execute();

                Assert.Equal(2, executed);
            }

            [Unit]
            public void GivenOnExecuteNotInvokedWhenExecuteAsyncIsInvokedThenThrowsException()
            {
                var app = new Fake.FakeExecutable();
                var ct = new CancellationToken();

                Assert.IsType<InvalidOperationException>(Record.ExceptionAsync(async () => await app.ExecuteAsync(ct)).Result);
            }

            [Unit]
            public async void WhenExecuteAsyncIsInvokedThenActionIsInvoked()
            {
                var executed = false;
                var app = new Fake.FakeExecutable();
                var cts = new CancellationTokenSource();
                app.OnExecute(() => { return true; });

                executed = await app.ExecuteAsync(cts.Token);

                Assert.True(executed);
            }

            [Unit]
            public async void GivenFunctionIsRegisteredWhenExecuteAsyncIsInvokedThenFunctionIsInvoked()
            {
                var executed = false;
                var app = new Fake.FakeExecutable();
                var cts = new CancellationTokenSource();
                app.OnExecute((ct) => { executed = true; return Task.FromResult(false); });

                await app.ExecuteAsync(cts.Token);

                Assert.True(executed);
            }

            [Unit]
            public async void GivenBothActionAndFunctionAreRegisteredWhenExecuteAsyncIsInvokedThenFuncIsInvoked()
            {
                var executed = 0;
                var app = new Fake.FakeExecutable();
                var cts = new CancellationTokenSource();

                app.OnExecute((ct) => { executed = 1; return Task.FromResult(false); });
                app.OnExecute(() => { executed = 2; return false; });

                await app.ExecuteAsync(cts.Token);

                Assert.Equal(1, executed);
            }
        }

        public class GivenAnExecutableOfTContextTResult
        {
            [Unit]
            public void GivenOnExecuteNotInvokedWhenExecuteIsInvokedThenThrowsException()
            {
                var app = new Fake.FakeContextExecutable();

                Action act = () => app.Execute(false);

                Assert.IsType(typeof(InvalidOperationException), Record.Exception(act));
            }

            [Unit]
            public void GivenApplicationServicesIsNullWhenActionTServiceIsRegisteredThenThrowsException()
            {
                var app = new Fake.FakeContextExecutable();

                Action act = () => app.OnExecute<string>((service, boo) => { return false; });

                Assert.IsType(typeof(InvalidOperationException), Record.Exception(act));
            }

            [Unit]
            public void WhenExecuteIsInvokedThenActionIsInvoked()
            {
                var executed = false;
                var app = new Fake.FakeContextExecutable();
                app.OnExecute((boo) => { return true; });

                executed = app.Execute(false);

                Assert.True(executed);
            }

            [Unit]
            public void GivenFunctionIsRegisteredWhenExecuteIsInvokedThenFunctionIsInvoked()
            {
                var executed = false;
                var app = new Fake.FakeContextExecutable();
                var cts = new CancellationTokenSource();
                app.OnExecute((boo, ct) => { executed = true; return Task.FromResult(false); });

                app.Execute(false);

                Assert.True(executed);
            }

            [Unit]
            public void GivenBothActionAndFunctionAreRegisteredWhenExecuteIsInvokedThenActionIsInvoked()
            {
                var executed = 0;
                var app = new Fake.FakeContextExecutable();

                app.OnExecute((boo, ct) => { executed = 1; return Task.FromResult(false); });
                app.OnExecute((boo) => { executed = 2; return false; });

                app.Execute(false);

                Assert.Equal(2, executed);
            }

            [Unit]
            public void GivenOnExecuteNotInvokedWhenExecuteAsyncIsInvokedThenThrowsException()
            {
                var app = new Fake.FakeContextExecutable();
                var ct = new CancellationToken();

                Assert.IsType<InvalidOperationException>(Record.ExceptionAsync(async () => await app.ExecuteAsync(false, ct)).Result);
            }

            [Unit]
            public async void WhenExecuteAsyncIsInvokedThenActionIsInvoked()
            {
                var executed = false;
                var app = new Fake.FakeContextExecutable();
                var cts = new CancellationTokenSource();
                app.OnExecute((boo) => { return true; });

                executed = await app.ExecuteAsync(false, cts.Token);

                Assert.True(executed);
            }

            [Unit]
            public async void GivenFunctionIsRegisteredWhenExecuteAsyncIsInvokedThenFunctionIsInvoked()
            {
                var executed = false;
                var app = new Fake.FakeContextExecutable();
                var cts = new CancellationTokenSource();
                app.OnExecute((boo, ct) => { executed = true; return Task.FromResult(false); });

                await app.ExecuteAsync(false, cts.Token);

                Assert.True(executed);
            }

            [Unit]
            public async void GivenBothActionAndFunctionAreRegisteredWhenExecuteAsyncIsInvokedThenFuncIsInvoked()
            {
                var executed = 0;
                var app = new Fake.FakeContextExecutable();
                var cts = new CancellationTokenSource();

                app.OnExecute((boo, ct) => { executed = 1; return Task.FromResult(false); });
                app.OnExecute((boo) => { executed = 2; return false; });

                await app.ExecuteAsync(false, cts.Token);

                Assert.Equal(1, executed);
            }
        }
    }
}