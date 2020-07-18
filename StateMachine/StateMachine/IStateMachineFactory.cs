using System;
using System.Linq;
using Microsoft.Extensions.Options;

namespace StateMachines
{
    public interface IStateMachineFactory<TState>
    {
        IStateMachine<TState> Create(string name, TState state);
    }

    public class StateMachineFactory<TData> : IStateMachineFactory<TData>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IOptionsMonitor<StateMachineOptions<TData>> _optionsMonitor;

        public StateMachineFactory(
            IServiceProvider serviceProvider,
            IOptionsMonitor<StateMachineOptions<TData>> optionsMonitor)
        {
            _serviceProvider = serviceProvider;
            _optionsMonitor = optionsMonitor;
        }

        public IStateMachine<TData> Create(string name, TData data)
        {
            var options = _optionsMonitor.Get(name);

            var steps = options.Steps
                .Select(stepType => _serviceProvider.GetService(stepType))
                .Cast<IStateMachineStep<TData>>();

            return new StateMachine<TData>(steps, data);
        }
    }

    public interface IStateMachineFactory<TState, TData>
    {
        IStateMachine<TState, TData> Create(string name, TState step, TData data);
    }

    public class StateMachineFactory<TState, TData> : IStateMachineFactory<TState, TData>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IOptionsMonitor<StateMachineOptions<TState, TData>> _optionsMonitor;

        public StateMachineFactory(
            IServiceProvider serviceProvider,
            IOptionsMonitor<StateMachineOptions<TState, TData>> optionsMonitor)
        {
            _serviceProvider = serviceProvider;
            _optionsMonitor = optionsMonitor;
        }

        public IStateMachine<TState, TData> Create(string name, TState state, TData data)
        {
            var options = _optionsMonitor.Get(name);

            var steps = options.Steps
                .Select(stepType => _serviceProvider.GetService(stepType))
                .Cast<IStateMachineStep<TState, TData>>();

            return new StateMachine<TState, TData>(steps, state, data);
        }
    }
}
