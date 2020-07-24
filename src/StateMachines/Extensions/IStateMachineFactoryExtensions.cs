using Microsoft.Extensions.Options;

namespace StateMachines
{
    public static class IStateMachineFactoryExtensions
    {
        public static IStateMachine<int, TData> Create<TData>(this IStateMachineFactory factory, TData data)
        {
            return factory.Create(Options.DefaultName, data);
        }

        public static IStateMachine<int, TData> Create<TData>(this IStateMachineFactory factory, string name, TData data)
        {
            return factory.Create(name, 0, data);
        }

        public static IStateMachine<int, TData> Create<TData>(this IStateMachineFactory factory, int state, TData data)
        {
            return factory.Create(Options.DefaultName, state, data);
        }

        public static IStateMachine<TState, TData> Create<TState, TData>(this IStateMachineFactory factory, TState state, TData data)
            where TState : struct
        {
            return factory.Create(Options.DefaultName, state, data);
        }
    }
}
