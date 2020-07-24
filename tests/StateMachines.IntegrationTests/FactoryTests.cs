using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using StateMachines.Exceptions;

using Xunit;

namespace StateMachines.IntegratedTests
{
    public class FactoryTests
    {
        [Fact]
        public void Factory_Should_ThrowMissingStepException_When_StateMachineDoesNotHaveAnyStep()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddStateMachine<DummyData>(stateMachine =>
                    {
                    });
                })
                .Build();

            var factory = host.Services.GetService<IStateMachineFactory>();

            Assert.Throws<MissingStepException>(() => factory.Create(string.Empty, 0, new DummyData()));
        }

        [Fact]
        public void FactoryWithState_ShouldMissingStepException_Throw_When_StateMachineDoesNotHaveAnyStep()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddStateMachine<DummyState, DummyData>(stateMachine =>
                    {
                        stateMachine.SetEndState(DummyState.Done);
                    });
                })
                .Build();

            var factory = host.Services.GetService<IStateMachineFactory>();

            Assert.Throws<MissingStepException>(() => factory.Create(string.Empty, DummyState.Start, new DummyData()));
        }

        [Fact]
        public void FactoryWithState_ShouldMissingEndStateException_Throw_When_StateMachineDoesNotHaveAnyStep()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddStateMachine<DummyState, DummyData>(stateMachine =>
                    {
                        stateMachine.AddStep<CounterStepWithState>();
                    });
                })
                .Build();

            var factory = host.Services.GetService<IStateMachineFactory>();

            Assert.Throws<MissingEndStateException>(() => factory.Create(string.Empty, DummyState.Start, new DummyData()));
        }

        [Fact]
        public async Task Factory_ReturnStateMachine_AteTheProvidedState()
        {
            var factory = new ServiceCollection()
                .AddStateMachine<DummyData>(stateMachine =>
                {
                    stateMachine
                        .AddStep<CounterStep>()
                        .AddStep<CounterStep>()
                        .AddStep<CounterStep>()
                        .AddStep<CounterStep>();
                })
                .BuildServiceProvider()
                .GetService<IStateMachineFactory>();

            var data = new DummyData();
            var machine = factory.Create(2, data);

            Assert.Equal(2, machine.State);

            while (await machine.MoveNextAsync()) { }

            Assert.Equal(2, data.Value);
        }

        [Fact]
        public async Task Factory_WithState_ReturnStateMachine_AteTheProvidedState()
        {
            var factory = new ServiceCollection()
                .AddStateMachine<DummyState, DummyData>(stateMachine =>
                {
                    stateMachine
                        .AddStep<StartStep>()
                        .AddStep<MiddleStep>()
                        .SetEndState(DummyState.Done);
                })
                .BuildServiceProvider()
                .GetService<IStateMachineFactory>();

            var data = new DummyData();
            var machine = factory.Create(DummyState.Middle, data);

            Assert.Equal(DummyState.Middle, machine.State);

            while (await machine.MoveNextAsync()) { }

            Assert.Equal(1, data.Value);
        }

        [Fact]
        public async Task NamedStateMachine_Should_HaveDifferentSteps()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddStateMachine<DummyData>("1", stateMachine =>
                    {
                        stateMachine.AddStep<CounterStep>();
                    });

                    services.AddStateMachine<DummyData>("2", stateMachine =>
                    {
                        stateMachine.AddStep<DoubleCounterStep>();
                    });
                })
                .Build();

            var factory = host.Services.GetService<IStateMachineFactory>();

            var data1 = new DummyData();
            var machine1 = factory.Create("1", data1);

            while (await machine1.MoveNextAsync()) { }

            var data2 = new DummyData();
            var machine2 = factory.Create("2", data2);

            while (await machine2.MoveNextAsync()) { }

            Assert.Equal(1, data1.Value);
            Assert.Equal(2, data2.Value);
        }

        [Fact]
        public async Task NamedStateMachine_WithState_Should_HaveDifferentSteps()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddStateMachine<DummyState, DummyData>("1", stateMachine =>
                    {
                        stateMachine.AddStep<CounterStepWithState>()
                            .SetEndState(DummyState.Done);
                    });

                    services.AddStateMachine<DummyState, DummyData>("2", stateMachine =>
                    {
                        stateMachine.AddStep<DoubleCounterStepWithState>()
                            .SetEndState(DummyState.Done);
                    });
                })
                .Build();

            var factory = host.Services.GetService<IStateMachineFactory>();

            var data1 = new DummyData();
            var machine1 = factory.Create("1", DummyState.Start, data1);

            while (await machine1.MoveNextAsync()) { }

            var data2 = new DummyData();
            var machine2 = factory.Create("2", DummyState.Start, data2);

            while (await machine2.MoveNextAsync()) { }

            Assert.Equal(1, data1.Value);
            Assert.Equal(2, data2.Value);
        }

        [Fact]
        public async Task StateMachineWithInstanceStep()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddStateMachine<DummyData>(stateMachine =>
                    {
                        stateMachine.AddStep(new InstanceStep(30));
                    });
                })
                .Build();

            var factory = host.Services.GetService<IStateMachineFactory>();

            var data = new DummyData();
            var machine = factory.Create(data);

            while (await machine.MoveNextAsync()) { }

            Assert.Equal(30, data.Value);
        }

        [Fact]
        public async Task StateMachineWithInstanceStep_Should_DifferenciateBetweenMultipleInstancesOfTheSameType()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddStateMachine<DummyData>(stateMachine =>
                    {
                        stateMachine
                            .AddStep(new InstanceStep(30))
                            .AddStep(new InstanceStep(15));
                    });
                })
                .Build();

            var factory = host.Services.GetService<IStateMachineFactory>();

            var data = new DummyData();
            var machine = factory.Create(data);

            while (await machine.MoveNextAsync()) { }

            Assert.Equal(45, data.Value);
        }

        [Fact]
        public async Task StateMachineWithInstanceStep_Should_DifferenciateBetweenMultipleInstancesOfTheSameType_AndDifferentNames()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddStateMachine<DummyData>("1", stateMachine =>
                    {
                        stateMachine.AddStep(new InstanceStep(30));
                    });
                    services.AddStateMachine<DummyData>("2", stateMachine =>
                    {
                        stateMachine.AddStep(new InstanceStep(15));
                    });
                })
                .Build();

            var factory = host.Services.GetService<IStateMachineFactory>();

            var data1 = new DummyData();
            var machine1 = factory.Create("1", data1);

            while (await machine1.MoveNextAsync()) { }

            Assert.Equal(30, data1.Value);

            var data2 = new DummyData();
            var machine2 = factory.Create("2", data2);

            while (await machine2.MoveNextAsync()) { }

            Assert.Equal(15, data2.Value);
        }

        public class DummyData
        {
            public int Value { get; set; }
        }

        public enum DummyState
        {
            Start,
            Middle,
            Done
        }

        public class InstanceStep : IStateMachineStep<DummyData>
        {
            private readonly int _value;

            public InstanceStep(int value)
            {
                _value = value;
            }

            public Task<bool> ExecuteAsync(DummyData data, CancellationToken cancellationToken = default)
            {
                data.Value += _value;
                return Task.FromResult(true);
            }
        }

        public class CounterStep : IStateMachineStep<DummyData>
        {
            public Task<bool> ExecuteAsync(DummyData data, CancellationToken cancellationToken = default)
            {
                data.Value += 1;
                return Task.FromResult(true);
            }
        }

        public class DoubleCounterStep : IStateMachineStep<DummyData>
        {
            public Task<bool> ExecuteAsync(DummyData data, CancellationToken cancellationToken = default)
            {
                data.Value += 2;
                return Task.FromResult(true);
            }
        }

        public class CounterStepWithState : IStateMachineStep<DummyState, DummyData>
        {
            public DummyState State => DummyState.Start;

            public Task<bool> ExecuteAsync(DummyData data, CancellationToken cancellationToken = default)
            {
                data.Value += 1;
                return Task.FromResult(true);
            }
        }

        public class StartStep : IStateMachineStep<DummyState, DummyData>
        {
            public DummyState State => DummyState.Start;

            public Task<bool> ExecuteAsync(DummyData data, CancellationToken cancellationToken = default)
            {
                data.Value += 1;
                return Task.FromResult(true);
            }
        }

        public class MiddleStep : IStateMachineStep<DummyState, DummyData>
        {
            public DummyState State => DummyState.Middle;

            public Task<bool> ExecuteAsync(DummyData data, CancellationToken cancellationToken = default)
            {
                data.Value += 1;
                return Task.FromResult(true);
            }
        }

        public class DoubleCounterStepWithState : IStateMachineStep<DummyState, DummyData>
        {
            public DummyState State => DummyState.Start;

            public Task<bool> ExecuteAsync(DummyData data, CancellationToken cancellationToken = default)
            {
                data.Value += 2;
                return Task.FromResult(true);
            }
        }
    }
}
