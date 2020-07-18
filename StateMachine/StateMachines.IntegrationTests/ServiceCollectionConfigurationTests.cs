using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Xunit;

namespace StateMachines.IntegratedTests
{
    public class ServiceCollectionConfigurationTests
    {
        [Fact]
        public async Task AddStateMachine_Should_Provider()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddStateMachine<DummyState>(stateMachine =>
                    {
                        stateMachine.AddStep<CounterStep>();
                    });
                })
                .Build();

            var factory = host.Services.GetService<IStateMachineFactory>();

            var state = new DummyState();
            var stateMachine = factory.Create(string.Empty, state);

            Assert.True(await stateMachine.MoveNextAsync());
            Assert.False(await stateMachine.MoveNextAsync());
            Assert.False(await stateMachine.MoveNextAsync());
            Assert.Equal(1, state.StepCount);
        }

        public class DummyState
        {
            public int StepCount { get; set; }
        }

        public class CounterStep : IStateMachineStep<DummyState>
        {
            public Task<bool> ExecuteAsync(DummyState data, CancellationToken cancellationToken = default)
            {
                data.StepCount += 1;
                return Task.FromResult(true);
            }
        }
    }
}
