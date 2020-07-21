using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StateMachines;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStateMachines(this IServiceCollection services)
        {
            services.TryAddScoped<IStateMachineFactory, StateMachineFactory>();

            return services;
        }

        public static IServiceCollection AddStateMachine<TData>(this IServiceCollection service, Action<IStateMachineBuilder<TData>> configure)
        {
            service.AddStateMachines();

            configure(new StateMachineBuilder<int, TData>(service, string.Empty));

            return service;
        }

        public static IServiceCollection AddStateMachine<TData>(this IServiceCollection service, string name, Action<IStateMachineBuilder<TData>> configure)
        {
            service.AddStateMachines();

            configure(new StateMachineBuilder<int, TData>(service, name));

            return service;
        }

        public static IServiceCollection AddStateMachine<TState, TData>(this IServiceCollection service, Action<IStateMachineBuilder<TState, TData>> configure) where TState : struct
        {
            service.AddStateMachines();

            configure(new StateMachineBuilder<TState, TData>(service, string.Empty));

            return service;
        }

        public static IServiceCollection AddStateMachine<TState, TData>(this IServiceCollection service, string name, Action<IStateMachineBuilder<TState, TData>> configure) where TState : struct
        {
            service.AddStateMachines();

            configure(new StateMachineBuilder<TState, TData>(service, name));

            return service;
        }
    }
}
