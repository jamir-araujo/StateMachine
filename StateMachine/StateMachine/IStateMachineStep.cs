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

    internal class IntegerStateWrapper<TData> : IStateMachineStep<int, TData>
    {
        private readonly IStateMachineStep<TData> _step;

        public IntegerStateWrapper(IStateMachineStep<TData> step, int state)
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

    internal class FinishingStep<TState, TData> : IStateMachineStep<TState, TData>
    {
        public FinishingStep(TState state)
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
