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

        public IStateMachine<TState, TData> Create<TState, TData>(string name, TState state, TData data) where TState : struct
        {
            var options = _serviceProvider
                .GetRequiredService<IOptionsMonitor<StateMachineOptions<TState, TData>>>()
                .Get(name);

            if (!options.Steps.Any())
            {
                throw new MissingStepException();
            }

            var steps = options.GetSteps(_serviceProvider);

            return new StateMachine<TState, TData>(steps, state, data);
        }
    }
}
