using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace StateMachine
{

    public interface IStateMachineBuilder<TState>
    {
        IServiceCollection Services { get; }
        IStateMachineBuilder<TState> AddStep<TStep>(ServiceLifetime lifetime = ServiceLifetime.Scoped) where TStep : IStateMachineStep<TState>;
        IStateMachineBuilder<TState> AddStep<TStep>(TStep step) where TStep : IStateMachineStep<TState>;
    }

    internal class StateMachineBuilder<TState> : IStateMachineBuilder<TState>
    {
        public StateMachineBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }

        public IStateMachineBuilder<TState> AddStep<TImplementation>(ServiceLifetime lifetime = ServiceLifetime.Scoped) where TImplementation : IStateMachineStep<TState>
        {
            Services.Add(ServiceDescriptor.Describe(typeof(IStateMachineStep<TState>), typeof(TState), lifetime));

            return this;
        }

        public IStateMachineBuilder<TState> AddStep<TImplementation>(TImplementation step) where TImplementation : IStateMachineStep<TState>
        {
            Services.AddSingleton<IStateMachineStep<TState>>(step);

            return this;
        }
    }

    public interface IStateMachineBuilder<TStep, TState>
    {
        IServiceCollection Services { get; }
        IStateMachineBuilder<TStep, TState> AddStep<TImplementation>(ServiceLifetime lifetime = ServiceLifetime.Scoped) where TImplementation : IStateMachineStep<TState>;
        IStateMachineBuilder<TStep, TState> AddStep<TImplementation>(TImplementation step) where TImplementation : IStateMachineStep<TStep, TState>;
    }

    public class StateMachineBuilder<TStep, TState> : IStateMachineBuilder<TStep, TState>
    {
        public StateMachineBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }

        public IStateMachineBuilder<TStep, TState> AddStep<TImplementation>(ServiceLifetime lifetime = ServiceLifetime.Scoped) where TImplementation : IStateMachineStep<TState>
        {
            Services.Add(ServiceDescriptor.Describe(typeof(IStateMachineStep<TStep, TState>), typeof(TState), lifetime));

            return this;
        }

        public IStateMachineBuilder<TStep, TState> AddStep<TImplementation>(TImplementation step) where TImplementation : IStateMachineStep<TStep, TState>
        {
            Services.AddSingleton<IStateMachineStep<TStep, TState>>(step);

            return this;
        }
    }
}
