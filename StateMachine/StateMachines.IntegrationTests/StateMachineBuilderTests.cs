using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Text;
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

        public class DummyData
        {
            public int StepCount { get; set; }
        }

        public class CounterStep : IStateMachineStep<DummyData>
        {
            public Task<bool> ExecuteAsync(DummyData data, CancellationToken cancellationToken = default)
            {
                data.StepCount += 1;
                return Task.FromResult(true);
            }
        }
    }
}
