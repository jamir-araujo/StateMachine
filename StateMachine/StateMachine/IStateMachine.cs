using System.Threading;
using System.Threading.Tasks;

namespace StateMachines
{
    public interface IStateMachine<TState, TData>
    {
        TState State { get; }
        TData Data { get; }
        Task<bool> MoveNextAsync(CancellationToken cancellationToken = default);
    }
}
