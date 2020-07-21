using System;
using System.Collections.Generic;

namespace StateMachines
{
    public class StateMachineOptions<TState, TData>
        where TState : struct
    {
        private readonly List<Type> _steps = new List<Type>();

        public IReadOnlyCollection<Type> Steps => _steps;
        public TState? EndState { get; set; }

        public void AddStep<TImplementation>()
        {
            _steps.Add(typeof(TImplementation));
        }
    }
}
