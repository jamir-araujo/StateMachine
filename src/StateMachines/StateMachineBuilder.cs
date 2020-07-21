using System;
using System.Collections.Generic;
using System.Linq;

using StateMachines.Exceptions;

namespace StateMachines
{
    public class StateMachineBuilder
    {
        public static IStateMachineBuilderStepAdder<TState, TData> Create<TState, TData>() where TState : struct
        {
            return new StateMachineBuilder<TState, TData>();
        }

        public static IStateMachineBuilderStepAdder<TData> Create<TData>()
        {
            return new StateMachineBuilder<TData>();
        }
    }

    public interface IStateMachineBuilderStepAdder<TState, TData> : IStateMachineBuilderEndState<TState, TData>
        where TState : struct
    {
        IStateMachineBuilderStepAdder<TState, TData> AddStep<TStep>(TStep step) where TStep : class, IStateMachineStep<TState, TData>;
        IStateMachineBuilderStepAdder<TState, TData> AddSteps<TStep>(IEnumerable<TStep> steps) where TStep : class, IStateMachineStep<TState, TData>;
    }

    public interface IStateMachineBuilderStepAdder<TData> : IStateMachineBuilder<int, TData>
    {
        IStateMachineBuilderStepAdder<TData> AddStep<TStep>(TStep step) where TStep : class, IStateMachineStep<TData>;
        IStateMachineBuilderStepAdder<TData> AddSteps<TStep>(IEnumerable<TStep> steps) where TStep : class, IStateMachineStep<TData>;
        IStateMachineBuilder<int, TData> SetEndState(int state);
    }

    public interface IStateMachineBuilderEndState<TState, TData>
        where TState : struct
    {
        IStateMachineBuilder<TState, TData> SetEndState(TState state);
    }

    public interface IStateMachineBuilder<TState, TData>
        where TState : struct
    {
        IStateMachine<TState, TData> Build(TState state, TData data);
    }

    internal class StateMachineBuilder<TData> :
        StateMachineBuilder<int, TData>,
        IStateMachineBuilderStepAdder<TData>
    {
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
            if (step is null)
            {
                throw new ArgumentNullException(nameof(step));
            }

            Steps.Add(new IntegerStateStepWrapper<TData>(step, Steps.Count));

            return this;
        }

        IStateMachineBuilderStepAdder<TData> IStateMachineBuilderStepAdder<TData>.AddSteps<TStep>(IEnumerable<TStep> steps)
        {
            if (steps is null)
            {
                throw new ArgumentNullException(nameof(steps));
            }

            foreach (var step in steps)
            {
                Steps.Add(new IntegerStateStepWrapper<TData>(step, Steps.Count));
            }

            return this;
        }
    }

    internal class StateMachineBuilder<TState, TData> :
        IStateMachineBuilderStepAdder<TState, TData>,
        IStateMachineBuilder<TState, TData>
        where TState : struct
    {
        protected readonly List<IStateMachineStep<TState, TData>> Steps = new List<IStateMachineStep<TState, TData>>();

        IStateMachineBuilderStepAdder<TState, TData> IStateMachineBuilderStepAdder<TState, TData>.AddStep<TStep>(TStep step)
        {
            if (step is null)
            {
                throw new ArgumentNullException(nameof(step));
            }

            Steps.Add(step);

            return this;
        }

        IStateMachineBuilderStepAdder<TState, TData> IStateMachineBuilderStepAdder<TState, TData>.AddSteps<TStep>(IEnumerable<TStep> steps)
        {
            if (steps is null)
            {
                throw new ArgumentNullException(nameof(steps));
            }

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
            if (!Steps.Any(s => !(s is EndStateStep<TState, TData>)))
            {
                throw new MissingStepException();
            }

            if (!Steps.OfType<EndStateStep<TState, TData>>().Any())
            {
                throw new MissingEndStateException(nameof(IStateMachine<TState, TData>));
            }

            return new StateMachine<TState, TData>(Steps, state, data);
        }
    }
}
