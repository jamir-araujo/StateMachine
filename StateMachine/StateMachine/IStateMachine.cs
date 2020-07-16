using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace StateMachine
{
    public interface IStateMachine<TState>
    {
        TState State { get; }
        Task<bool> MoveNextAsync();
    }

    public class StateMachine<TState> : IStateMachine<TState>
    {
        private readonly List<IStateMachineStep<TState>> _steps;
        private int _step;

        public StateMachine(IEnumerable<IStateMachineStep<TState>> steps, TState state)
        {
            State = state;

            _steps = steps.ToList();
            _step = 0;
        }

        public TState State { get; }

        public async Task<bool> MoveNextAsync()
        {
            if (await _steps[_step].ProcessAsync(State))
            {
                _step += 1;

                return true;
            }

            return false;
        }
    }

    public interface IStateMachine<TStep, TState> : IStateMachine<TState>
    {
        TStep Step { get; }
    }

    public class StateMachine<TStep, TState> : IStateMachine<TStep, TState>
    {
        public StateMachine(IEnumerable<IStateMachineStep<TStep, TState>> steps, TStep step, TState state)
        {
            Step = step;
            State = state;

            _steps = steps.GetEnumerator();
        }

        public TStep Step { get; private set; }
        public TState State { get; }

        private IEnumerator<IStateMachineStep<TStep, TState>> _steps;

        public async Task<bool> MoveNextAsync()
        {
            do
            {
                if (_steps.Current.Step.Equals(Step))
                {
                    if (await _steps.Current.ProcessAsync(State))
                    {
                        return _steps.MoveNext();
                    }
                }
            } while (_steps.MoveNext());

            return false;
        }
    }
}
