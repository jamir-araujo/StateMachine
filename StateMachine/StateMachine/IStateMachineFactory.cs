using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace StateMachines
{
    public interface IStateMachineFactory
    {
        IStateMachine<int, TState> Create<TState>(string name, TState state);
        IStateMachine<TState, TData> Create<TState, TData>(string name, TState state, TData data);
    }

    public class StateMachineFactory : IStateMachineFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public StateMachineFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IStateMachine<int, TData> Create<TData>(string name, TData data)
        {
            var optionsMonitor = _serviceProvider
                .GetRequiredService<IOptionsMonitor<StateMachineOptions<int, TData>>>();

            var options = optionsMonitor.Get(name);

            var steps = options.Steps
                .Select(stepType => _serviceProvider.GetRequiredService(stepType))
                .Cast<IStateMachineStep<TData>>()
                .Select((step, i) => new IntegerStateWrapper<TData>(step, i))
                .Cast<IStateMachineStep<int, TData>>()
                .Concat(new[] { new FinishingStep<int, TData>(options.Steps.Count) });

            return new StateMachine<int, TData>(steps, 0, data);
        }

        public IStateMachine<TState, TData> Create<TState, TData>(string name, TState state, TData data)
        {
            var optionsMonitor = _serviceProvider
                .GetRequiredService<IOptionsMonitor<StateMachineOptions<TState, TData>>>();

            var options = optionsMonitor.Get(name);

            var steps = options.Steps
                .Select(stepType => _serviceProvider.GetService(stepType))
                .Cast<IStateMachineStep<TState, TData>>();

            return new StateMachine<TState, TData>(steps, state, data);
        }
    }
}
