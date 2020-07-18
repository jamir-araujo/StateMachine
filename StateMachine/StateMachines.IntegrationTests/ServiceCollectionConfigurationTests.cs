using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace StateMachines.IntegratedTests
{
    public class ServiceCollectionConfigurationTests
    {
        [Fact]
        public async Task AddStateMachineAsync()
        {
            var services = new ServiceCollection();

            services.AddStateMachine<DummyState>(stateMachine =>
            {
                stateMachine.AddStep<CounterStep>();
            });

            var provider = services.BuildServiceProvider();

            var factory = provider.GetService<IStateMachineFactory>();

            var stateMachine = factory.Create(string.Empty, new DummyState());

            while (await stateMachine.MoveNextAsync())
            {

            }
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
