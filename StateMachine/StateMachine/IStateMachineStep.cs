using System.Threading;
using System.Threading.Tasks;

namespace StateMachines
{
    public interface IStateMachineStep<TData>
    {
        Task<bool> ExecuteAsync(TData data, CancellationToken cancellationToken = default);
    }

    public interface IStateMachineStep<TState, TData> : IStateMachineStep<TData>
    {
        TState State { get; }
    }

    internal class IntegerStateStepWrapper<TData> : IStateMachineStep<int, TData>
    {
        private readonly IStateMachineStep<TData> _step;

        public IntegerStateStepWrapper(IStateMachineStep<TData> step, int state)
        {
            State = state;

            _step = step;
        }

        public int State { get; }

        public Task<bool> ExecuteAsync(TData data, CancellationToken cancellationToken = default)
        {
            return _step.ExecuteAsync(data, cancellationToken);
        }
    }

    internal class EndStateStep<TState, TData> : IStateMachineStep<TState, TData>
    {
        public EndStateStep(TState state)
        {
            State = state;
        }

        public TState State { get; }

        public Task<bool> ExecuteAsync(TData data, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }
    }
}
