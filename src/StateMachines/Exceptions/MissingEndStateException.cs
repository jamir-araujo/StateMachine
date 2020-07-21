using System;

namespace StateMachines.Exceptions
{
    public class MissingEndStateException : InvalidOperationException
    {
        public MissingEndStateException(string stateMachineType)
            : base($"Cannot build state machine of type {stateMachineType} without an EndState.")
        {
        }
    }
}
