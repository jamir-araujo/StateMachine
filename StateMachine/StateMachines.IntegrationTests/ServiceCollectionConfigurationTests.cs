using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using StateMachines.Exceptions;

using Xunit;

namespace StateMachines.IntegratedTests
{
    public class ServiceCollectionConfigurationTests
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

        public class DummyData
        {
            public int StepCount { get; set; }
        }

        public enum DummyState
        {
            Start,
            Middle,
            Done
        }

        public class CounterStep : IStateMachineStep<DummyData>
        {
            public Task<bool> ExecuteAsync(DummyData data, CancellationToken cancellationToken = default)
            {
                data.StepCount += 1;
                return Task.FromResult(true);
            }
        }

        public class CounterStepWithState : IStateMachineStep<DummyState, DummyData>
        {
            public DummyState State => DummyState.Start;

            public Task<bool> ExecuteAsync(DummyData data, CancellationToken cancellationToken = default)
            {
                data.StepCount += 1;
                return Task.FromResult(true);
            }
        }
    }
}
