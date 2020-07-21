
using StateMachines;

namespace Microsoft.Extensions.DependencyInjection
{
    internal class StateMachineBuilder<TState, TData> : IStateMachineBuilder<TState, TData>, IStateMachineBuilder<TData>
        where TState : struct
    {
        public StateMachineBuilder(IServiceCollection services, string name)
        {
            Services = services;
            Name = name;
        }

        public IServiceCollection Services { get; }
        public string Name { get; }

        IStateMachineBuilder<TState, TData> IStateMachineBuilder<TState, TData>.AddStep<TImplementation>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            Services.Add(ServiceDescriptor.Describe(typeof(TImplementation), typeof(TImplementation), lifetime));

            AddStep<TImplementation>();

            return this;
        }

        IStateMachineBuilder<TState, TData> IStateMachineBuilder<TState, TData>.AddStep<TImplementation>(TImplementation step)
        {
            Services.AddSingleton(step);

            AddStep<TImplementation>();

            return this;
        }

        IStateMachineBuilder<TState, TData> IStateMachineBuilder<TState, TData>.SetEndState(TState state)
        {
            Services.Configure<StateMachineOptions<TState, TData>>(Name, o => o.EndState = state);

            return this;
        }

        IStateMachineBuilder<TData> IStateMachineBuilder<TData>.SetEndState(int state)
        {
            Services.Configure<StateMachineOptions<int, TData>>(Name, o => o.EndState = state);

            return this;
        }

        IStateMachineBuilder<TData> IStateMachineBuilder<TData>.AddStep<TImplementation>(ServiceLifetime lifetime)
        {
            Services.Add(ServiceDescriptor.Describe(typeof(TImplementation), typeof(TImplementation), lifetime));

            AddStep<TImplementation>();

            return this;
        }

        IStateMachineBuilder<TData> IStateMachineBuilder<TData>.AddStep<TImplementation>(TImplementation step)
        {
            Services.AddSingleton(step);

            AddStep<TImplementation>();

            return this;
        }

        private void AddStep<TImplementation>()
        {
            Services.Configure<StateMachineOptions<TState, TData>>(Name, o => o.AddStep<TImplementation>());
        }
    }
}
