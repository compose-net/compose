using FluentAssertions;
using System;
using System.Threading;
using System.Threading.Tasks;
using TestAttributes;

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

                    Action act = () => app.ExecuteAsync(ct).Wait();

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
                public async void WithBothActionAndFunctionRegistrationsThenFunctionIsInvoked()
                {
                    var executed = 0;
                    var app = new Executable();
                    var cts = new CancellationTokenSource();

                    app.OnExecute((ct) => { executed = 1; return Task.FromResult(false); });
                    app.OnExecute(() => { executed = 2; });

                    await app.ExecuteAsync(cts.Token);

                    executed.Should().Be(1);
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

        public class GivenAnExecutableOfTResult
        {
            public class WhenExecuteIsInvoked
            {
                [Unit]
                public void WithOnExecuteNotInvokedThenThrowsException()
                {
                    var app = new Fake.FakeExecutable();

                    Action act = () => app.Execute();

                    act.ShouldThrow<InvalidOperationException>();
                }

                [Unit]
                public void ThenActionIsInvoked()
                {
                    var executed = false;
                    var app = new Fake.FakeExecutable();
                    app.OnExecute(() => { executed = true; return true; });

                    app.Execute();

                    executed.Should().BeTrue();
                }

                [Unit]
                public void ThenRegisteredFunctionIsInvoked()
                {
                    var executed = false;
                    var app = new Fake.FakeExecutable();
                    var cts = new CancellationTokenSource();
                    app.OnExecute((ct) => { executed = true; return Task.FromResult(false); });

                    app.Execute();

                    executed.Should().BeTrue();
                }

                [Unit]
                public void WithBothActionAndFunctionRegistrationsThenActionIsInvoked()
                {
                    var executed = 0;
                    var app = new Fake.FakeExecutable();

                    app.OnExecute((ct) => { executed = 1; return Task.FromResult(false); });
                    app.OnExecute(() => { executed = 2; return true; });

                    app.Execute();

                    executed.Should().Be(2);
                }
            }

            public class WhenExecuteAsyncIsInvoked
            {
                [Unit]
                public void WithOnExecuteNotInvokedThenThrowsException()
                {
                    var app = new Fake.FakeExecutable();
                    var ct = new CancellationToken();

                    Action act = () => app.ExecuteAsync(ct).Wait();

                    act.ShouldThrow<InvalidOperationException>();
                }

                [Unit]
                public async void ThenActionIsInvoked()
                {
                    var executed = false;
                    var app = new Fake.FakeExecutable();
                    var cts = new CancellationTokenSource();
                    app.OnExecute(() => { executed = true; return true; });

                    await app.ExecuteAsync(cts.Token);

                    executed.Should().BeTrue();
                }

                [Unit]
                public async void ThenRegisteredFunctionIsInvoked()
                {
                    var executed = false;
                    var app = new Fake.FakeExecutable();
                    var cts = new CancellationTokenSource();
                    app.OnExecute((ct) => { executed = true; return Task.FromResult(false); });

                    await app.ExecuteAsync(cts.Token);

                    executed.Should().BeTrue();
                }

                [Unit]
                public async void WithBothActionAndFunctionRegistrationsThenFunctionIsInvoked()
                {
                    var executed = 0;
                    var app = new Fake.FakeExecutable();
                    var cts = new CancellationTokenSource();

                    app.OnExecute((ct) => { executed = 1; return Task.FromResult(false); });
                    app.OnExecute(() => { executed = 2; return true; });

                    await app.ExecuteAsync(cts.Token);

                    executed.Should().Be(1);
                }
            }

            public class WhenOnExecuteIsInvoked
            {
                [Unit]
                public void WithNoServicesRegisteredThenThrowsException()
                {
                    var app = new Fake.FakeExecutable();

                    Action act = () => app.OnExecute<string>((service) => true);

                    act.ShouldThrow<InvalidOperationException>();
                }
            }
        }

        public class GivenAnExecutableOfTContextTResult
        {
            public class WhenExecuteIsInvoked
            {
                [Unit]
                public void WithOnExecuteNotInvokedThenThrowsException()
                {
                    var app = new Fake.FakeContextExecutable();

                    Action act = () => app.Execute(true);

                    act.ShouldThrow<InvalidOperationException>();
                }

                [Unit]
                public void ThenActionIsInvoked()
                {
                    var executed = false;
                    var app = new Fake.FakeContextExecutable();
                    app.OnExecute((b) => { executed = true; return true; });

                    app.Execute(true);

                    executed.Should().BeTrue();
                }

                [Unit]
                public void ThenRegisteredFunctionIsInvoked()
                {
                    var executed = false;
                    var app = new Fake.FakeContextExecutable();
                    var cts = new CancellationTokenSource();
                    app.OnExecute((ct, b) => { executed = true; return Task.FromResult(false); });

                    app.Execute(true);

                    executed.Should().BeTrue();
                }

                [Unit]
                public void WithBothActionAndFunctionRegistrationsThenActionIsInvoked()
                {
                    var executed = 0;
                    var app = new Fake.FakeContextExecutable();

                    app.OnExecute((ct, b) => { executed = 1; return Task.FromResult(false); });
                    app.OnExecute((b) => { executed = 2; return true; });

                    app.Execute(true);

                    executed.Should().Be(2);
                }
            }

            public class WhenExecuteAsyncIsInvoked
            {
                [Unit]
                public void WithOnExecuteNotInvokedThenThrowsException()
                {
                    var app = new Fake.FakeContextExecutable();
                    var ct = new CancellationToken();

                    Action act = () => app.ExecuteAsync(true, ct).Wait();

                    act.ShouldThrow<InvalidOperationException>();
                }

                [Unit]
                public async void ThenActionIsInvoked()
                {
                    var executed = false;
                    var app = new Fake.FakeContextExecutable();
                    var cts = new CancellationTokenSource();
                    app.OnExecute(b => { executed = true; return true; });

                    await app.ExecuteAsync(true, cts.Token);

                    executed.Should().BeTrue();
                }

                [Unit]
                public async void ThenRegisteredFunctionIsInvoked()
                {
                    var executed = false;
                    var app = new Fake.FakeContextExecutable();
                    var cts = new CancellationTokenSource();
                    app.OnExecute((ct, b) => { executed = true; return Task.FromResult(false); });

                    await app.ExecuteAsync(true, cts.Token);

                    executed.Should().BeTrue();
                }

                [Unit]
                public async void WithBothActionAndFunctionRegistrationsThenFunctionIsInvoked()
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

            public class WhenOnExecuteIsInvoked
            {
                [Unit]
                public void WithNoServicesRegisteredThenThrowsException()
                {
                    var app = new Fake.FakeContextExecutable();

                    Action act = () => app.OnExecute<string>((service, b) => { return true; });

                    act.ShouldThrow<InvalidOperationException>();
                }
            }
        }
    }
}