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

        public static IServiceCollection AddStateMachine<TState>(this IServiceCollection service, Action<IStateMachineBuilder<TState>> configure)
        {
            service.AddStateMachines();

            configure(new StateMachineBuilder<TState>(service, string.Empty));

            return service;
        }

        public static IServiceCollection AddStateMachine<TState>(this IServiceCollection service, string name, Action<IStateMachineBuilder<TState>> configure)
        {
            service.AddStateMachines();

            configure(new StateMachineBuilder<TState>(service, name));

            return service;
        }

        public static IServiceCollection AddStateMachine<TStep, TState>(this IServiceCollection service, Action<IStateMachineBuilder<TStep, TState>> configure)
        {
            service.AddStateMachines();

            configure(new StateMachineBuilder<TStep, TState>(service, string.Empty));

            return service;
        }

        public static IServiceCollection AddStateMachine<TStep, TState>(this IServiceCollection service, string name, Action<IStateMachineBuilder<TStep, TState>> configure)
        {
            service.AddStateMachines();

            configure(new StateMachineBuilder<TStep, TState>(service, name));

            return service;
        }
    }
}
