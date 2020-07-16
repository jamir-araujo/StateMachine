using System;
using System.Collections.Generic;

namespace StateMachine
{
    public interface IStateMachineFactory<TState>
    {
        IStateMachine<TState> Crate(TState state);
    }

    public class StateMachineFactory<TState> : IStateMachineFactory<TState>
    {
        private readonly IEnumerable<IStateMachineStep<TState>> _steps;

        public StateMachineFactory(IEnumerable<IStateMachineStep<TState>> steps)
        {
            _steps = steps;
        }

        public IStateMachine<TState> Crate(TState state)
        {
            return new StateMachine<TState>(_steps, state);
        }
    }

    public interface IStateMachineFactory<TStep, TState>
    {
        IStateMachine<TStep, TState> Crate(TStep step, TState state);
    }

    public class StateMachineFactory<TStep, TState> : IStateMachineFactory<TStep, TState>
    {
        private readonly IEnumerable<IStateMachineStep<TStep, TState>> _steps;

        public StateMachineFactory(IEnumerable<IStateMachineStep<TStep, TState>> steps)
        {
            _steps = steps;
        }

        public IStateMachine<TStep, TState> Crate(TStep step, TState state)
        {
            return new StateMachine<TStep, TState>(_steps, step, state);
        }
    }
}
