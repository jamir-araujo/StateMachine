using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace StateMachines.Tests
{
    public class StateMachineStateDataTests
    {
        [Fact]
        public async Task MoveNextAsync_Should_ReturnFalseAtFirstCall_When_StepsAreEmpty()
        {
            var state = new DummyData();
            var machine = new StateMachine<int, DummyData>(Enumerable.Empty<IStateMachineStep<int, DummyData>>(), 0, state);

            Assert.False(await machine.MoveNextAsync());
        }

        [Fact]
        public async Task MoveNextAsync_Should_CallAllSteps_When_AllReturnTrue()
        {
            var stepMock = new Mock<IStateMachineStep<int, DummyData>>();

            var steps = new List<IStateMachineStep<int, DummyData>>
            {
                stepMock.Object,
                stepMock.Object,
                stepMock.Object,
                stepMock.Object
            };

            var state = new DummyData();
            var machine = new StateMachine<int, DummyData>(steps, 0, state);

            stepMock
                .Setup(s => s.ExecuteAsync(state, default))
                .ReturnsAsync(true);

            while (await machine.MoveNextAsync()) { }

            stepMock.Verify(s => s.ExecuteAsync(state, default), Times.Exactly(steps.Count));
        }

        [Fact]
        public async Task MoveNextAsync_Should_ReturnFalse_When_FirstStepReturnsFalse()
        {
            var trueStepMock = new Mock<IStateMachineStep<int, DummyData>>();
            var falseStepMock = new Mock<IStateMachineStep<int, DummyData>>();

            var steps = new List<IStateMachineStep<int, DummyData>>
            {
                falseStepMock.Object,
                trueStepMock.Object,
                trueStepMock.Object,
                trueStepMock.Object
            };

            var state = new DummyData();
            var machine = new StateMachine<int, DummyData>(steps, 0, state);

            trueStepMock
                .Setup(s => s.ExecuteAsync(state, default))
                .ReturnsAsync(true);

            Assert.False(await machine.MoveNextAsync());
        }

        [Fact]
        public async Task MoveNextAsync_Should_Interrupt_When_OneStepReturnsFalse()
        {
            var trueStepMock = new Mock<IStateMachineStep<int, DummyData>>();
            var falseStepMock = new Mock<IStateMachineStep<int, DummyData>>();

            var steps = new List<IStateMachineStep<int, DummyData>>
            {
                trueStepMock.Object,
                trueStepMock.Object,
                falseStepMock.Object,
                trueStepMock.Object
            };

            var state = new DummyData();
            var machine = new StateMachine<int, DummyData>(steps, 0, state);

            trueStepMock
                .Setup(s => s.ExecuteAsync(state, default))
                .ReturnsAsync(true);

            var stepCount = 0;
            while (await machine.MoveNextAsync())
            {
                stepCount++;
            }

            trueStepMock.Verify(s => s.ExecuteAsync(state, default), Times.Exactly(2));
            falseStepMock.Verify(s => s.ExecuteAsync(state, default), Times.Once);
            Assert.Equal(2, stepCount);
        }

        [Fact]
        public async Task MoveNextAsync_Should_KeepExecutingTheSameStepUntilReturnsTrue()
        {
            var falseStep = new SettableStep(2, false);
            var steps = new List<IStateMachineStep<int, DummyData>>
            {
                new SettableStep(0,true),
                new SettableStep(1, true),
                falseStep,
                new SettableStep(3,true),
                new SettableStep(4,true)
            };

            var state = new DummyData();
            var machine = new StateMachine<int, DummyData>(steps, 0, state);

            Assert.True(await machine.MoveNextAsync());
            Assert.True(await machine.MoveNextAsync());

            Assert.False(await machine.MoveNextAsync());
            Assert.False(await machine.MoveNextAsync());
            Assert.False(await machine.MoveNextAsync());
            Assert.False(await machine.MoveNextAsync());
            Assert.False(await machine.MoveNextAsync());

            falseStep.Success = true;

            Assert.True(await machine.MoveNextAsync());
            Assert.True(await machine.MoveNextAsync());
            Assert.False(await machine.MoveNextAsync());

            //finished

            Assert.False(await machine.MoveNextAsync());
            Assert.False(await machine.MoveNextAsync());
            Assert.False(await machine.MoveNextAsync());

        }

        [Fact]
        public async Task NewStateMachine_Should_BeAbleToContinueFromGivenState()
        {
            var falseStep = new SettableStep(2, false);
            var steps = new List<IStateMachineStep<int, DummyData>>
            {
                new SettableStep(0,true),
                new SettableStep(1, true),
                falseStep,
                new SettableStep(3,true),
                new SettableStep(4,true)
            };

            var data = new DummyData();
            var machine = new StateMachine<int, DummyData>(steps, 0, data);

            Assert.True(await machine.MoveNextAsync());
            Assert.True(await machine.MoveNextAsync());
            Assert.False(await machine.MoveNextAsync());

            falseStep.Success = true;

            var state = machine.State;
            var newMachine = new StateMachine<int, DummyData>(steps, state, data);

            Assert.True(await newMachine.MoveNextAsync());
            Assert.True(await newMachine.MoveNextAsync());
            Assert.False(await newMachine.MoveNextAsync());

            Assert.False(await newMachine.MoveNextAsync());
            Assert.False(await newMachine.MoveNextAsync());
        }

        [Fact]
        public async Task NewStateMachine_Should_NotExecuteSteps_When_StartsWithLastStepCompeleted()
        {
            var steps = new List<IStateMachineStep<int, DummyData>>
            {
                new SettableStep(0, true),
                new SettableStep(1, true),
                new SettableStep(2, true),
                new SettableStep(3, true),
                new SettableStep(4, true)
            };

            var data = new DummyData();

            var machine = new StateMachine<int, DummyData>(steps, 0, data);

            Assert.True(await machine.MoveNextAsync());
            Assert.True(await machine.MoveNextAsync());
            Assert.True(await machine.MoveNextAsync());
            Assert.True(await machine.MoveNextAsync());
            Assert.False(await machine.MoveNextAsync());

            var stepMock = new Mock<IStateMachineStep<int, DummyData>>();

            stepMock
                .Setup(s => s.ExecuteAsync(data, default))
                .ReturnsAsync(true);

            stepMock
                .Setup(s => s.State)
                .Returns(4);

            var state = machine.State;
            steps[4] = stepMock.Object;

            var newMachine = new StateMachine<int, DummyData>(steps, state, data);

            Assert.False(await newMachine.MoveNextAsync());

            stepMock.Verify(s => s.ExecuteAsync(data, default), Times.Never);
        }

        public class SettableStep : IStateMachineStep<int, DummyData>
        {
            public bool Success { get; set; }
            public int State { get; }

            public SettableStep(int state, bool success)
            {
                State = state;
                Success = success;
            }

            public Task<bool> ExecuteAsync(DummyData data, CancellationToken cancellationToken = default)
            {
                return Task.FromResult(Success);
            }
        }

        public class DummyData
        {

        }
    }
}
