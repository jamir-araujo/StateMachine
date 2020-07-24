using System;

namespace StateMachines.Exceptions
{
    public class MissingStepException : InvalidOperationException
    {
        public MissingStepException()
            : base("StateMachine must have at least one step")
        {

        }
    }
}
