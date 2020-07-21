namespace StateMachines
{
    public static class IStateMachineBuilderExtensions
    {
        public static IStateMachine<int, TData> Build<TData>(this IStateMachineBuilder<int, TData> builder, TData data)
        {
            return builder.Build(0, data);
        }
    }
}
