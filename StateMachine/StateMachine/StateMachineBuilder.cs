using System.Collections.Generic;
using System.Linq;

namespace StateMachines
{
    public interface IStateMachineBuilderAddStep<TState, TData> : IStateMachineBuilderEndState<TState, TData>
    {
        IStateMachineBuilderAddStep<TState, TData> AddStep<TStep>(TStep step) where TStep : class, IStateMachineStep<TState, TData>;
        IStateMachineBuilderAddStep<TState, TData> AddSteps<TStep>(IEnumerable<TStep> steps) where TStep : class, IStateMachineStep<TState, TData>;
    }

    public interface IStateMachineBuilderAddStep<TData> : IStateMachineBuilder<int, TData>
    {
        IStateMachineBuilderAddStep<TData> AddStep<TStep>(TStep step) where TStep : class, IStateMachineStep<TData>;
        IStateMachineBuilderAddStep<TData> AddSteps<TStep>(IEnumerable<TStep> steps) where TStep : class, IStateMachineStep<TData>;
        IStateMachine<int, TData> Build(TData data);

    }

    public interface IStateMachineBuilderEndState<TState, TData>
    {
        IStateMachineBuilder<TState, TData> SetEndState(TState state);
    }

    public interface IStateMachineBuilder<TState, TData>
    {
        IStateMachine<TState, TData> Build(TState state, TData data);
    }

    public class StateMachineBuilder
    {
        public static IStateMachineBuilderAddStep<TState, TData> Create<TState, TData>()
        {
            return new StateMachineBuilder<TState, TData>();
        }

        public static IStateMachineBuilderAddStep<TData> Create<TData>()
        {
            return new StateMachineBuilder<TData>();
        }
    }

    public class StateMachineBuilder<TData> : IStateMachineBuilder<int, TData>, IStateMachineBuilderAddStep<TData>
    {
        private readonly List<IStateMachineStep<TData>> _steps = new List<IStateMachineStep<TData>>();

        public IStateMachineBuilderAddStep<TData> AddStep<TStep>(TStep step) where TStep : class, IStateMachineStep<TData>
        {
            _steps.Add(step);

            return this;
        }

        public IStateMachineBuilderAddStep<TData> AddSteps<TStep>(IEnumerable<TStep> steps) where TStep : class, IStateMachineStep<TData>
        {
            _steps.AddRange(steps);

            return this;
        }

        public IStateMachine<int, TData> Build(TData data)
        {
            return Build(0, data);
        }

        public IStateMachine<int, TData> Build(int state, TData data)
        {
            var steps = _steps.Select((s, i) => new IntegerStateStepWrapper<TData>(s, i))
                .Cast<IStateMachineStep<int, TData>>()
                .Concat(new[] { new EndStateStep<int, TData>(_steps.Count) });

            return new StateMachine<int, TData>(steps, state, data);
        }
    }

    public class StateMachineBuilder<TState, TData> : IStateMachineBuilderAddStep<TState, TData>, IStateMachineBuilder<TState, TData>
    {
        private readonly List<IStateMachineStep<TState, TData>> _steps = new List<IStateMachineStep<TState, TData>>();

        public IStateMachineBuilderAddStep<TState, TData> AddStep<TStep>(TStep step) where TStep : class, IStateMachineStep<TState, TData>
        {
            _steps.Add(step);

            return this;
        }

        public IStateMachineBuilderAddStep<TState, TData> AddSteps<TStep>(IEnumerable<TStep> steps) where TStep : class, IStateMachineStep<TState, TData>
        {
            _steps.AddRange(steps);

            return this;
        }

        public IStateMachine<TState, TData> Build(TState state, TData data)
        {
            return new StateMachine<TState, TData>(_steps, state, data);
        }

        public IStateMachineBuilder<TState, TData> SetEndState(TState state)
        {
            _steps.Add(new EndStateStep<TState, TData>(state));

            return this;
        }
    }
}
