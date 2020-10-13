using System;

using Microsoft.Extensions.DependencyInjection;

namespace StateMachines
{
    public abstract class StepDescriptor<TState, TData>
        where TState : struct
    {
        public abstract IStateMachineStep<TState, TData> GetStep(IServiceProvider serviceProvider);
    }

    internal class StepDescriptor<TImplementation, TState, TData> : StepDescriptor<TState, TData>
        where TImplementation : IStateMachineStep<TState, TData>
        where TState : struct
    {
        private readonly TImplementation _instance;

        public StepDescriptor() { }

        public StepDescriptor(TImplementation instance) => _instance = instance;

        public override IStateMachineStep<TState, TData> GetStep(IServiceProvider serviceProvider)
        {
            if (_instance == null)
            {
                return serviceProvider.GetService<TImplementation>();
            }

            return _instance;
        }
    }

    internal class IntStepDescriptor<TImplementation, TData> : StepDescriptor<int, TData>
        where TImplementation : IStateMachineStep<TData>
    {
        private readonly TImplementation _instance;
        private readonly int _state;

        public IntStepDescriptor(int state) => _state = state;

        public IntStepDescriptor(TImplementation instance, int state)
            : this(state) => _instance = instance;

        public override IStateMachineStep<int, TData> GetStep(IServiceProvider serviceProvider)
        {
            return new IntegerStateStepWrapper<TData>(GetStateInternal(serviceProvider), _state);
        }

        private IStateMachineStep<TData> GetStateInternal(IServiceProvider serviceProvider)
        {
            if (_instance == null)
            {
                return serviceProvider.GetService<TImplementation>();
            }

            return _instance;
        }
    }

    internal class EndStepDescriptor<TState, TData> : StepDescriptor<TState, TData>
        where TState : struct
    {
        public EndStepDescriptor(TState state) => State = state;

        public TState State { get; }

        public override IStateMachineStep<TState, TData> GetStep(IServiceProvider serviceProvider)
        {
            return new EndStateStep<TState, TData>(State);
        }
    }
}
