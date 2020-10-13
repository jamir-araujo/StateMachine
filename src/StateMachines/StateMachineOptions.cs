using System;
using System.Collections.Generic;
using System.Linq;

namespace StateMachines
{
    public class StateMachineOptions<TState, TData>
        where TState : struct
    {
        private readonly List<StepDescriptor<TState, TData>> _steps = new List<StepDescriptor<TState, TData>>();
        private EndStepDescriptor<TState, TData> _endStepDescriptor;

        public IReadOnlyCollection<StepDescriptor<TState, TData>> Steps => _steps;
        public TState? EndState => _endStepDescriptor?.State;

        internal void AddStep(StepDescriptor<TState, TData> descriptor)
            => _steps.Add(descriptor);

        internal void SetEndStep(TState state)
            => _endStepDescriptor = new EndStepDescriptor<TState, TData>(state);

        internal IEnumerable<IStateMachineStep<TState, TData>> GetSteps(IServiceProvider serviceProvider)
        {
            var steps = _steps.AsEnumerable();

            if (_endStepDescriptor != null)
            {
                steps = steps.Concat(new[] { _endStepDescriptor });
            }

            return steps.Select(descriptor => descriptor.GetStep(serviceProvider));
        }
    }
}
