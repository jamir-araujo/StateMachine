using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace StateMachines.IntegratedTests
{
    public class StateMachineBuilderTests
    {
        [Fact]
        public async Task StateMachineBuiltWithBuilder_Should_RunTheFinalStep()
        {
            var dummy = new DummyData();

            var machine = StateMachineBuilder.Create<DummyData>()
                .AddStep(new CounterStep())
                .AddStep(new CounterStep())
                .AddStep(new CounterStep())
                .Build(dummy);

            Assert.Equal(0, machine.State);
            Assert.True(await machine.MoveNextAsync());

            Assert.Equal(1, machine.State);
            Assert.True(await machine.MoveNextAsync());

            Assert.Equal(2, machine.State);
            Assert.True(await machine.MoveNextAsync());

            Assert.Equal(3, machine.State);
            Assert.False(await machine.MoveNextAsync());

            Assert.Equal(3, machine.State);
            Assert.False(await machine.MoveNextAsync());
            Assert.Equal(3, machine.State);
            Assert.False(await machine.MoveNextAsync());
            Assert.Equal(3, machine.State);
            Assert.False(await machine.MoveNextAsync());

            Assert.Equal(3, dummy.StepCount);
        }

        [Fact]
        public async Task StateMachineBuiltWithBuilder_Should_BeAbleToStartFromAState()
        {
            var dummy = new DummyData();

            var machine = StateMachineBuilder.Create<DummyData>()
                .AddStep(new CounterStep())
                .AddStep(new CounterStep())
                .AddStep(new CounterStep())
                .AddStep(new CounterStep())
                .AddStep(new CounterStep())
                .Build(2, dummy);

            Assert.Equal(2, machine.State);
            Assert.True(await machine.MoveNextAsync());

            Assert.Equal(3, machine.State);
            Assert.True(await machine.MoveNextAsync());

            Assert.Equal(4, machine.State);
            Assert.True(await machine.MoveNextAsync());

            Assert.Equal(5, machine.State);
            Assert.False(await machine.MoveNextAsync());

            Assert.Equal(5, machine.State);
            Assert.False(await machine.MoveNextAsync());
            Assert.Equal(5, machine.State);
            Assert.False(await machine.MoveNextAsync());
            Assert.Equal(5, machine.State);
            Assert.False(await machine.MoveNextAsync());

            Assert.Equal(3, dummy.StepCount);
        }

        [Fact]
        public async Task Builder_Should_BeAbleToBuildANewStateMachineWithPreviousState()
        {
            var dummy = new DummyData();

            var builder = StateMachineBuilder.Create<DummyData>()
                .AddStep(new CounterStep())
                .AddStep(new CounterStep())
                .AddStep(new CounterStep())
                .AddStep(new CounterStep())
                .AddStep(new CounterStep());

            var machine1 = builder.Build(dummy);

            Assert.Equal(0, machine1.State);
            Assert.True(await machine1.MoveNextAsync());

            Assert.Equal(1, machine1.State);
            Assert.True(await machine1.MoveNextAsync());

            var machine2 = builder.Build(machine1.State, dummy);

            Assert.Equal(2, machine2.State);
            Assert.True(await machine2.MoveNextAsync());

            Assert.Equal(3, machine2.State);
            Assert.True(await machine2.MoveNextAsync());

            Assert.Equal(4, machine2.State);
            Assert.True(await machine2.MoveNextAsync());

            Assert.Equal(5, machine2.State);
            Assert.False(await machine2.MoveNextAsync());
        }

        [Fact]
        public async Task BuilderWithState_Should_BeAbleToBuildANewStateMachineWithPreviousState()
        {
            var dummy = new DummyData();

            var builder = StateMachineBuilder.Create<DummyState, DummyData>()
                .AddStep(new StartStep())
                .AddStep(new MiddleStep())
                .SetEndState(DummyState.Done);

            var machine1 = builder.Build(DummyState.Start, dummy);

            Assert.Equal(DummyState.Start, machine1.State);
            Assert.True(await machine1.MoveNextAsync());

            Assert.Equal(DummyState.Middle, machine1.State);

            var machine2 = builder.Build(machine1.State, dummy);

            Assert.Equal(DummyState.Middle, machine1.State);
            Assert.True(await machine2.MoveNextAsync());

            Assert.Equal(DummyState.Done, machine2.State);
            Assert.False(await machine2.MoveNextAsync());
        }

        public class DummyData
        {
            public int StepCount { get; set; }
        }

        public enum DummyState
        {
            Start,
            Middle,
            Done
        }

        public class CounterStep : IStateMachineStep<DummyData>
        {
            public Task<bool> ExecuteAsync(DummyData data, CancellationToken cancellationToken = default)
            {
                data.StepCount += 1;
                return Task.FromResult(true);
            }
        }

        public class StartStep : IStateMachineStep<DummyState, DummyData>
        {
            public DummyState State => DummyState.Start;

            public Task<bool> ExecuteAsync(DummyData data, CancellationToken cancellationToken = default)
            {
                return Task.FromResult(true);
            }
        }

        public class MiddleStep : IStateMachineStep<DummyState, DummyData>
        {
            public DummyState State => DummyState.Middle;

            public Task<bool> ExecuteAsync(DummyData data, CancellationToken cancellationToken = default)
            {
                return Task.FromResult(true);
            }
        }
    }
}
