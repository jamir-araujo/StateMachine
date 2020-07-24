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

        public static IServiceCollection AddStateMachine<TData>(this IServiceCollection services, Action<IStateMachineBuilder<TData>> configure)
        {
            services.AddStateMachines();

            configure(new StateMachineBuilder<int, TData>(services, Options.Options.DefaultName));

            return services;
        }

        public static IServiceCollection AddStateMachine<TData>(this IServiceCollection services, string name, Action<IStateMachineBuilder<TData>> configure)
        {
            services.AddStateMachines();

            configure(new StateMachineBuilder<int, TData>(services, name));

            return services;
        }

        public static IServiceCollection AddStateMachine<TState, TData>(this IServiceCollection services, Action<IStateMachineBuilder<TState, TData>> configure) where TState : struct
        {
            services.AddStateMachines();

            configure(new StateMachineBuilder<TState, TData>(services, Options.Options.DefaultName));

            return services;
        }

        public static IServiceCollection AddStateMachine<TState, TData>(this IServiceCollection services, string name, Action<IStateMachineBuilder<TState, TData>> configure) where TState : struct
        {
            services.AddStateMachines();

            configure(new StateMachineBuilder<TState, TData>(services, name));

            return services;
        }
    }
}
