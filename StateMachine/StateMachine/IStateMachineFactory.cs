using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace StateMachines
{
    public interface IStateMachineFactory
    {
        IStateMachine<TState> Create<TState>(string name, TState state);
        IStateMachine<TState, TData> Create<TState, TData>(string name, TState step, TData data);
    }

    public class StateMachineFactory : IStateMachineFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public StateMachineFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IStateMachine<TData> Create<TData>(string name, TData data)
        {
            var optionsMonitor = _serviceProvider
                .GetRequiredService<IOptionsMonitor<StateMachineOptions<TData>>>();

            var options = optionsMonitor.Get(name);

            var steps = options.Steps
                .Select(stepType => _serviceProvider.GetService(stepType))
                .Cast<IStateMachineStep<TData>>();

            return new StateMachine<TData>(steps, data);
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
