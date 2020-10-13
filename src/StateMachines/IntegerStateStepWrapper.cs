using System.Threading;
using System.Threading.Tasks;

namespace StateMachines
{
    internal class IntegerStateStepWrapper<TData> : IStateMachineStep<int, TData>
    {
        private readonly IStateMachineStep<TData> _step;

        public IntegerStateStepWrapper(IStateMachineStep<TData> step, int state)
        {
            State = state;

            _step = step;
        }

        public int State { get; }

        public Task<bool> ExecuteAsync(TData data, CancellationToken cancellationToken = default)
            => _step.ExecuteAsync(data, cancellationToken);
    }
}
