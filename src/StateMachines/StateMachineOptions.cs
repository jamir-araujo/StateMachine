using System;
using System.Collections.Generic;

namespace StateMachines
{
    public class StateMachineOptions<TState, TData>
        where TState : struct
    {
        private readonly List<StepDescriptor> _steps = new List<StepDescriptor>();

        public IReadOnlyCollection<StepDescriptor> Steps => _steps;
        public TState? EndState { get; set; }

        public void AddStep<TImplementation>()
        {
            _steps.Add(new StepDescriptor(typeof(TImplementation)));
        }

        public void AddStep<TImplementation>(TImplementation step)
        {
            _steps.Add(new StepDescriptor(step));
        }
    }

    public class StepDescriptor
    {
        public Type ServiceType { get; set; }
        public object ImplementationInstance { get; set; }

        public StepDescriptor(Type serviceType)
        {
            ServiceType = serviceType;
        }

        public StepDescriptor(object implementationInstance)
        {
            ImplementationInstance = implementationInstance;
        }

        public object GetInstance(IServiceProvider serviceProvider)
        {
            if (!(ImplementationInstance is null))
            {
                return ImplementationInstance;
            }

            return serviceProvider.GetService(ServiceType);
        }
    }
}
