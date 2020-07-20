using System;

using StateMachines;

namespace Microsoft.Extensions.DependencyInjection
{
    public interface IStateMachineBuilder<TState, TData>
    {
        string Name { get; }
        IServiceCollection Services { get; }
        IStateMachineBuilder<TState, TData> AddStep<TImplementation>(ServiceLifetime lifetime = ServiceLifetime.Scoped) where TImplementation : class, IStateMachineStep<TData>;
        IStateMachineBuilder<TState, TData> AddStep<TImplementation>(TImplementation step) where TImplementation : class, IStateMachineStep<TState, TData>;
        IStateMachineBuilder<TState, TData> SetEndState(TState state);
    }

    public class StateMachineBuilder<TState, TData> : IStateMachineBuilder<TState, TData>
    {
        public StateMachineBuilder(IServiceCollection services, string name)
        {
            Services = services;
            Name = name;
        }

        public IServiceCollection Services { get; }
        public string Name { get; }

        public IStateMachineBuilder<TState, TData> AddStep<TImplementation>(ServiceLifetime lifetime = ServiceLifetime.Scoped) where TImplementation : class, IStateMachineStep<TData>
        {
            Services.Add(ServiceDescriptor.Describe(typeof(TImplementation), typeof(TImplementation), lifetime));

            AddStep<TImplementation>();

            return this;
        }

        public IStateMachineBuilder<TState, TData> AddStep<TImplementation>(TImplementation step) where TImplementation : class, IStateMachineStep<TState, TData>
        {
            Services.AddSingleton(step);

            AddStep<TImplementation>();

            return this;
        }

        public IStateMachineBuilder<TState, TData> SetEndState(TState state)
        {
            Services.Configure<StateMachineOptions<TState, TData>>(Name, o => o.EndState = state);

            return this;
        }

        private void AddStep<TImplementation>()
        {
            Services.Configure<StateMachineOptions<TState, TData>>(Name, o => o.AddStep<TImplementation>());
        }
    }
}
