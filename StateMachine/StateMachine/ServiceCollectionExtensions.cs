using Microsoft.Extensions.DependencyInjection;

namespace StateMachine
{
    public static class ServiceCollectionExtensions
    {
        public static IStateMachineBuilder<TState> AddStateMachine<TState>(this IServiceCollection service)
        {
            return new StateMachineBuilder<TState>(service);
        }

        public static IStateMachineBuilder<TStep, TState> AddStateMachine<TStep, TState>(this IServiceCollection service)
        {
            return new StateMachineBuilder<TStep, TState>(service);
        }
    }
}
