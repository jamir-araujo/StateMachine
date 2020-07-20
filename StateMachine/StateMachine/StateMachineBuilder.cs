using System;
using System.Collections.Generic;
using System.Linq;

using StateMachines.Exceptions;

namespace StateMachines
{
    public interface IStateMachineBuilderStepAdder<TState, TData> : IStateMachineBuilderEndState<TState, TData>
    {
        IStateMachineBuilderStepAdder<TState, TData> AddStep<TStep>(TStep step) where TStep : class, IStateMachineStep<TState, TData>;
        IStateMachineBuilderStepAdder<TState, TData> AddSteps<TStep>(IEnumerable<TStep> steps) where TStep : class, IStateMachineStep<TState, TData>;
    }

    public interface IStateMachineBuilderStepAdder<TData> : IStateMachineBuilder<TData>
    {
        IStateMachineBuilderStepAdder<TData> AddStep<TStep>(TStep step) where TStep : class, IStateMachineStep<TData>;
        IStateMachineBuilderStepAdder<TData> AddSteps<TStep>(IEnumerable<TStep> steps) where TStep : class, IStateMachineStep<TData>;
        IStateMachineBuilder<TData> SetEndState(int state);
    }

    public interface IStateMachineBuilderEndState<TState, TData>
    {
        IStateMachineBuilder<TState, TData> SetEndState(TState state);
    }

    public interface IStateMachineBuilder<TState, TData>
    {
        IStateMachine<TState, TData> Build(TState state, TData data);
    }

    public interface IStateMachineBuilder<TData> : IStateMachineBuilder<int, TData>
    {
        IStateMachine<int, TData> Build(TData data);
    }

    public class StateMachineBuilder
    {
        public static IStateMachineBuilderStepAdder<TState, TData> Create<TState, TData>()
        {
            return new StateMachineBuilder<TState, TData>();
        }

        public static IStateMachineBuilderStepAdder<TData> Create<TData>()
        {
            return new StateMachineBuilder<TData>();
        }
    }

    internal class StateMachineBuilder<TData> :
        StateMachineBuilder<int, TData>,
        IStateMachineBuilder<TData>,
        IStateMachineBuilderStepAdder<TData>
    {
        public IStateMachine<int, TData> Build(TData data)
        {
            return Build(0, data);
        }

        public override IStateMachine<int, TData> Build(int state, TData data)
        {
            if (!Steps.Any())
            {
                throw new MissingStepException();
            }

            if (!Steps.OfType<EndStateStep<int, TData>>().Any())
            {
                Steps.Add(new EndStateStep<int, TData>(Steps.Count));
            }

            return new StateMachine<int, TData>(Steps, state, data);
        }

        IStateMachineBuilderStepAdder<TData> IStateMachineBuilderStepAdder<TData>.AddStep<TStep>(TStep step)
        {
            Steps.Add(new IntegerStateStepWrapper<TData>(step, Steps.Count));

            return this;
        }

        IStateMachineBuilderStepAdder<TData> IStateMachineBuilderStepAdder<TData>.AddSteps<TStep>(IEnumerable<TStep> steps)
        {
            foreach (var step in steps)
            {
                Steps.Add(new IntegerStateStepWrapper<TData>(step, Steps.Count));
            }

            return this;
        }

        IStateMachineBuilder<TData> IStateMachineBuilderStepAdder<TData>.SetEndState(int state)
        {
            SetEndState(state);

            return this;
        }
    }

    internal class StateMachineBuilder<TState, TData> :
        IStateMachineBuilderStepAdder<TState, TData>,
        IStateMachineBuilder<TState, TData>
    {
        protected readonly List<IStateMachineStep<TState, TData>> Steps = new List<IStateMachineStep<TState, TData>>();

        public IStateMachineBuilderStepAdder<TState, TData> AddStep<TStep>(TStep step) where TStep : class, IStateMachineStep<TState, TData>
        {
            Steps.Add(step);

            return this;
        }

        public IStateMachineBuilderStepAdder<TState, TData> AddSteps<TStep>(IEnumerable<TStep> steps) where TStep : class, IStateMachineStep<TState, TData>
        {
            Steps.AddRange(steps);

            return this;
        }

        public IStateMachineBuilder<TState, TData> SetEndState(TState state)
        {
            Steps.Add(new EndStateStep<TState, TData>(state));

            return this;
        }

        public virtual IStateMachine<TState, TData> Build(TState state, TData data)
        {
            if (!Steps.Any())
            {
                throw new MissingStepException();
            }

            if (!Steps.OfType<EndStateStep<TState, TData>>().Any())
            {
                throw new MissingEndStateException(nameof(EndStateStep<TState, TData>));
            }

            return new StateMachine<TState, TData>(Steps, state, data);
        }
    }
}
