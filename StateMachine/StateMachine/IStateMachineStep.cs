using System.Threading.Tasks;

namespace StateMachine
{
    public interface IStateMachineStep<TState>
    {
        Task<bool> ProcessAsync(TState state);
    }

    public interface IStateMachineStep<TStep, TState> : IStateMachineStep<TState>
    {
        TStep Step { get; }
    }
}
