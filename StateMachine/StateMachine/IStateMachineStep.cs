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
}
