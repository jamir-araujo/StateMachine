namespace StateMachines
{
    public interface IStateMachineFactory
    {
        IStateMachine<TState, TData> Create<TState, TData>(string name, TState state, TData data) where TState : struct;
    }
}
