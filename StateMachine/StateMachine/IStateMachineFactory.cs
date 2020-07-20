namespace StateMachines
{
    public interface IStateMachineFactory
    {
        IStateMachine<int, TData> Create<TData>(string name, int state, TData data);
        IStateMachine<TState, TData> Create<TState, TData>(string name, TState state, TData data);
    }
}
