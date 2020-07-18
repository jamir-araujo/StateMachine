using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace StateMachines
{

    public interface IStateMachineBuilder<TState>
    {
        string Name { get; }
        IServiceCollection Services { get; }
        IStateMachineBuilder<TState> AddStep<TImplementation>(ServiceLifetime lifetime = ServiceLifetime.Scoped) where TImplementation : IStateMachineStep<TState>;
        IStateMachineBuilder<TState> AddStep<TImplementation>(TImplementation step) where TImplementation : class, IStateMachineStep<TState>;
    }

    internal class StateMachineBuilder<TData> : IStateMachineBuilder<TData>
    {
        public StateMachineBuilder(IServiceCollection services, string name)
        {
            Services = services;
            Name = name;
        }

        public IServiceCollection Services { get; }
        public string Name { get; }

        public IStateMachineBuilder<TData> AddStep<TImplementation>(ServiceLifetime lifetime = ServiceLifetime.Scoped) where TImplementation : IStateMachineStep<TData>
        {
            Services.Add(ServiceDescriptor.Describe(typeof(TImplementation), typeof(TImplementation), lifetime));

            AddStep<TImplementation>();

            return this;
        }

        public IStateMachineBuilder<TData> AddStep<TImplementation>(TImplementation step) where TImplementation : class, IStateMachineStep<TData>
        {
            Services.AddSingleton(step);

            AddStep<TImplementation>();

            return this;
        }

        private void AddStep<TImplementation>() => Services.Configure<StateMachineOptions<TData>>(Name, o => o.AddStep<TImplementation>());
    }

    public interface IStateMachineBuilder<TStep, TState>
    {
        string Name { get; }
        IServiceCollection Services { get; }
        IStateMachineBuilder<TStep, TState> AddStep<TImplementation>(ServiceLifetime lifetime = ServiceLifetime.Scoped) where TImplementation : IStateMachineStep<TState>;
        IStateMachineBuilder<TStep, TState> AddStep<TImplementation>(TImplementation step) where TImplementation : IStateMachineStep<TStep, TState>;
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

        public IStateMachineBuilder<TStep, TState> AddStep<TImplementation>(ServiceLifetime lifetime = ServiceLifetime.Scoped) where TImplementation : IStateMachineStep<TState>
        {
            Services.Add(ServiceDescriptor.Describe(typeof(IStateMachineStep<TStep, TState>), typeof(TState), lifetime));

            AddStep<TImplementation>();

            return this;
        }

        public IStateMachineBuilder<TStep, TState> AddStep<TImplementation>(TImplementation step) where TImplementation : IStateMachineStep<TStep, TState>
        {
            Services.AddSingleton<IStateMachineStep<TStep, TState>>(step);

            AddStep<TImplementation>();

            return this;
        }

        private void AddStep<TImplementation>() => Services.Configure<StateMachineOptions<TStep, TState>>(Name, o => o.AddStep<TImplementation>());
    }

    public class StateMachineOptions<TState>
    {
        private readonly List<Type> _steps = new List<Type>();

        public IReadOnlyCollection<Type> Steps => _steps;

        public void AddStep<TStep>()
        {
            _steps.Add(typeof(TStep));
        }
    }

    public class StateMachineOptions<TStep, TState> : StateMachineOptions<TState>
    {
    }
}
