using System;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using StateMachines.Exceptions;

namespace StateMachines
{
    internal class StateMachineFactory : IStateMachineFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public StateMachineFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IStateMachine<int, TData> Create<TData>(string name, int state, TData data)
        {
            var optionsMonitor = _serviceProvider.GetRequiredService<IOptionsMonitor<StateMachineOptions<int, TData>>>();

            var options = optionsMonitor.Get(name);

            var steps = options.Steps
                .Select(stepType => _serviceProvider.GetRequiredService(stepType))
                .Cast<IStateMachineStep<TData>>();

            return StateMachineBuilder.Create<TData>()
                .AddSteps(steps)
                .Build(state, data);
        }

        public IStateMachine<TState, TData> Create<TState, TData>(string name, TState state, TData data) where TState : struct
        {
            var optionsMonitor = _serviceProvider.GetRequiredService<IOptionsMonitor<StateMachineOptions<TState, TData>>>();

            var options = optionsMonitor.Get(name);

            var steps = options.Steps
                .Select(stepType => _serviceProvider.GetRequiredService(stepType))
                .Cast<IStateMachineStep<TState, TData>>();

            if (!options.EndState.HasValue)
            {
                throw new MissingEndStateException(nameof(IStateMachine<TState, TData>));
            }

            return StateMachineBuilder.Create<TState, TData>()
                .AddSteps(steps)
                .SetEndState(options.EndState.Value)
                .Build(state, data);
        }
    }
}
