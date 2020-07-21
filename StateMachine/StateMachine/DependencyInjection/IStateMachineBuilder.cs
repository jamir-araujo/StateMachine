using System;

using StateMachines;

namespace Microsoft.Extensions.DependencyInjection
{
    public interface IStateMachineBuilder
    {
        string Name { get; }
        IServiceCollection Services { get; }
    }

    public interface IStateMachineBuilder<TState, TData> : IStateMachineBuilder
    {
        IStateMachineBuilder<TState, TData> AddStep<TImplementation>(ServiceLifetime lifetime = ServiceLifetime.Scoped) where TImplementation : class, IStateMachineStep<TState, TData>;
        IStateMachineBuilder<TState, TData> AddStep<TImplementation>(TImplementation step) where TImplementation : class, IStateMachineStep<TState, TData>;
        IStateMachineBuilder<TState, TData> SetEndState(TState state);
    }

    public interface IStateMachineBuilder<TData> : IStateMachineBuilder
    {
        IStateMachineBuilder<TData> AddStep<TImplementation>(ServiceLifetime lifetime = ServiceLifetime.Scoped) where TImplementation : class, IStateMachineStep<TData>;
        IStateMachineBuilder<TData> AddStep<TImplementation>(TImplementation step) where TImplementation : class, IStateMachineStep<TData>;
        IStateMachineBuilder<TData> SetEndState(int state);
    }
}
