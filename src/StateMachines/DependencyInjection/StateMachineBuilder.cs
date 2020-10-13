
using System;

using StateMachines;

namespace Microsoft.Extensions.DependencyInjection
{
    internal abstract class StateMachineBuilderBase<TState, TData>
        where TState : struct
    {
        public StateMachineBuilderBase(IServiceCollection services, string name)
        {
            Services = services;
            Name = name;

            Services.AddOptions<StateMachineOptions<TState, TData>>(name)
                .Validate(options => options.Steps.Count > 0, "StateMachine must have at least one step");
        }

        public IServiceCollection Services { get; }
        public string Name { get; }
    }

    internal class StateMachineBuilder<TState, TData> : StateMachineBuilderBase<TState, TData>, IStateMachineBuilder<TState, TData>
        where TState : struct
    {
        public StateMachineBuilder(IServiceCollection services, string name)
            : base(services, name)
        {
            Services.AddOptions<StateMachineOptions<TState, TData>>(Name)
                .Validate(options => options.EndState.HasValue, "StateMachine must have an end state");
        }

        IStateMachineBuilder<TState, TData> IStateMachineBuilder<TState, TData>.AddStep<TImplementation>(ServiceLifetime lifetime)
        {
            Services.Add(ServiceDescriptor.Describe(typeof(TImplementation), typeof(TImplementation), lifetime));

            Configure(o => o.AddStep(new StepDescriptor<TImplementation, TState, TData>()));

            return this;
        }

        IStateMachineBuilder<TState, TData> IStateMachineBuilder<TState, TData>.AddStep<TImplementation>(TImplementation step)
        {
            Configure(o => o.AddStep(new StepDescriptor<TImplementation, TState, TData>(step)));

            return this;
        }

        IStateMachineBuilder<TState, TData> IStateMachineBuilder<TState, TData>.SetEndState(TState state)
        {
            Configure(o => o.SetEndStep(state));

            return this;
        }

        private void Configure(Action<StateMachineOptions<TState, TData>> configure)
            => Services.Configure(Name, configure);
    }

    internal class StateMachineBuilder<TData> : StateMachineBuilderBase<int, TData>, IStateMachineBuilder<TData>
    {
        public StateMachineBuilder(IServiceCollection services, string name)
            : base(services, name)
        {
            services.AddOptions<StateMachineOptions<int, TData>>(name)
                .PostConfigure(options =>
                {
                    if (!options.EndState.HasValue)
                    {
                        options.SetEndStep(options.Steps.Count);
                    }
                });
        }

        IStateMachineBuilder<TData> IStateMachineBuilder<TData>.SetEndState(int state)
        {
            Configure(o => o.SetEndStep(state));

            return this;
        }

        IStateMachineBuilder<TData> IStateMachineBuilder<TData>.AddStep<TImplementation>(ServiceLifetime lifetime)
        {
            Services.Add(ServiceDescriptor.Describe(typeof(TImplementation), typeof(TImplementation), lifetime));

            Configure(o => o.AddStep(new IntStepDescriptor<TImplementation, TData>(o.Steps.Count)));

            return this;
        }

        IStateMachineBuilder<TData> IStateMachineBuilder<TData>.AddStep<TImplementation>(TImplementation step)
        {
            Configure(o => o.AddStep(new IntStepDescriptor<TImplementation, TData>(step, o.Steps.Count)));

            return this;
        }

        private void Configure(Action<StateMachineOptions<int, TData>> configure)
            => Services.Configure(Name, configure);
    }
}
