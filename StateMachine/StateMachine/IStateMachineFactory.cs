using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;

namespace StateMachine
{
    public interface IStateMachineFactory<TState>
    {
        IStateMachine<TState> Crate(string name, TState state);
    }

    public class StateMachineFactory<TState> : IStateMachineFactory<TState>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IOptionsMonitor<StateMachineOptions<TState>> _optionsMonitor;

        public StateMachineFactory(
            IServiceProvider serviceProvider,
            IOptionsMonitor<StateMachineOptions<TState>> optionsMonitor)
        {
            _serviceProvider = serviceProvider;
            _optionsMonitor = optionsMonitor;
        }

        public IStateMachine<TState> Crate(string name, TState state)
        {
            var options = _optionsMonitor.Get(name);

            var steps = options.Steps
                .Select(stepType => _serviceProvider.GetService(stepType))
                .Cast<IStateMachineStep<TState>>();

            return new StateMachine<TState>(steps, state);
        }
    }

    public interface IStateMachineFactory<TStep, TState>
    {
        IStateMachine<TStep, TState> Crate(string name, TStep step, TState state);
    }

    public class StateMachineFactory<TStep, TState> : IStateMachineFactory<TStep, TState>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IOptionsMonitor<StateMachineOptions<TStep, TState>> _optionsMonitor;

        public StateMachineFactory(
            IServiceProvider serviceProvider,
            IOptionsMonitor<StateMachineOptions<TStep, TState>> optionsMonitor)
        {
            _serviceProvider = serviceProvider;
            _optionsMonitor = optionsMonitor;
        }

        public IStateMachine<TStep, TState> Crate(string name, TStep step, TState state)
        {
            var options = _optionsMonitor.Get(name);

            var steps = options.Steps
                .Select(stepType => _serviceProvider.GetService(stepType))
                .Cast<IStateMachineStep<TStep, TState>>();

            return new StateMachine<TStep, TState>(steps, step, state);
        }
    }
}
