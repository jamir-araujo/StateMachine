using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace StateMachines
{
    public interface IStateMachine<TData>
    {
        TData Data { get; }
        Task<bool> MoveNextAsync(CancellationToken cancellationToken = default);
    }

    public class StateMachine<TData> : IStateMachine<TData>
    {
        private readonly IEnumerator<IStateMachineStep<TData>> _steps;

        public StateMachine(IEnumerable<IStateMachineStep<TData>> steps, TData state)
        {
            Data = state;

            _steps = steps.GetEnumerator();
        }

        public TData Data { get; }

        public async Task<bool> MoveNextAsync(CancellationToken cancellationToken = default)
        {
            while (_steps.MoveNext())
            {
                return await _steps.Current.ExecuteAsync(Data, cancellationToken);
            }

            return false;
        }
    }

    public interface IStateMachine<TState, TData> : IStateMachine<TData>
    {
        TState State { get; }
    }

    public class StateMachine<TState, TData> : IStateMachine<TState, TData>
    {
        public StateMachine(IEnumerable<IStateMachineStep<TState, TData>> steps, TState step, TData data)
        {
            State = step;
            Data = data;

            _steps = steps.GetEnumerator();
        }

        public TState State { get; private set; }
        public TData Data { get; }

        private readonly IEnumerator<IStateMachineStep<TState, TData>> _steps;

        public async Task<bool> MoveNextAsync(CancellationToken cancellationToken = default)
        {
            if (TryMoveToState())
            {
                if (await _steps.Current.ExecuteAsync(Data))
                {
                    if (_steps.MoveNext())
                    {
                        State = _steps.Current.State;
                        return true;
                    }
                }
            }

            return false;
        }

        private bool TryMoveToState()
        {
            if (_steps.Current == null)
            {
                if (!_steps.MoveNext())
                {
                    return false;
                }
            }

            do
            {
                if (_steps.Current.State.Equals(State))
                {
                    return true;
                }
            }
            while (_steps.MoveNext());

            return false;
        }

        class FinalStep : IStateMachineStep<TState, TData>
        {
            public TState State => default;

            public Task<bool> ExecuteAsync(TData data, CancellationToken cancellationToken = default)
            {
                return Task.FromResult(false);
            }
        }
    }
}
