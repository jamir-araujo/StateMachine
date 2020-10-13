using System.Threading;
using System.Threading.Tasks;

namespace StateMachines
{
    internal class EndStateStep<TState, TData> : IStateMachineStep<TState, TData>
        where TState : struct
    {
        public EndStateStep(TState state) => State = state;

        public TState State { get; }

        public Task<bool> ExecuteAsync(TData data, CancellationToken cancellationToken = default)
            => Task.FromResult(false);
    }
}
