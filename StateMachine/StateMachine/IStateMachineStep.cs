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

    internal class WrapperStep<TData> : IStateMachineStep<int, TData>
    {
        private readonly IStateMachineStep<TData> _step;

        public WrapperStep(IStateMachineStep<TData> step, int state)
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
}
