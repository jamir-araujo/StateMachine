using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace StateMachines
{
    public interface IStateMachineBuilder<TStep, TState>
    {
        string Name { get; }
        IServiceCollection Services { get; }
        IStateMachineBuilder<TStep, TState> AddStep<TImplementation>(ServiceLifetime lifetime = ServiceLifetime.Scoped) where TImplementation : class, IStateMachineStep<TState>;
        IStateMachineBuilder<TStep, TState> AddStep<TImplementation>(TImplementation step) where TImplementation : class, IStateMachineStep<TStep, TState>;
    }

    public class StateMachineBuilder<TStep, TState> : IStateMachineBuilder<TStep, TState>
    {
        public StateMachineBuilder(IServiceCollection services, string name)
        {
            Services = services;
            Name = name;
        }

        public IServiceCollection Services { get; }
        public string Name { get; }

        public IStateMachineBuilder<TStep, TState> AddStep<TImplementation>(ServiceLifetime lifetime = ServiceLifetime.Scoped) where TImplementation : class, IStateMachineStep<TState>
        {
            Services.Add(ServiceDescriptor.Describe(typeof(TImplementation), typeof(TImplementation), lifetime));

            AddStep<TImplementation>();

            return this;
        }

        public IStateMachineBuilder<TStep, TState> AddStep<TImplementation>(TImplementation step) where TImplementation : class, IStateMachineStep<TStep, TState>
        {
            Services.AddSingleton(step);

            AddStep<TImplementation>();

            return this;
        }

        private void AddStep<TImplementation>() => Services.Configure<StateMachineOptions<TStep, TState>>(Name, o => o.AddStep<TImplementation>());
    }

    public class StateMachineOptions<TStep, TState>
    {
        private readonly List<Type> _steps = new List<Type>();

        public IReadOnlyCollection<Type> Steps => _steps;

        public void AddStep<TImplementation>()
        {
            _steps.Add(typeof(TImplementation));
        }
    }
}
