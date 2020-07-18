using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace StateMachines.Tests
{
    public class StateMachineDataTests
    {
        [Fact]
        public async Task MoveNextAsync_Should_ReturnFalseAtFirstCall_When_StepsAreEmpty()
        {
            var state = new DummyState();
            var machine = new StateMachine<DummyState>(Enumerable.Empty<IStateMachineStep<DummyState>>(), state);

            Assert.False(await machine.MoveNextAsync());
        }

        [Fact]
        public async Task MoveNextAsync_Should_CallAllSteps_When_AllReturnTrue()
        {
            var stepMock = new Mock<IStateMachineStep<DummyState>>();

            var steps = new List<IStateMachineStep<DummyState>>
            {
                stepMock.Object,
                stepMock.Object,
                stepMock.Object,
                stepMock.Object
            };

            var state = new DummyState();
            var machine = new StateMachine<DummyState>(steps, state);

            stepMock
                .Setup(s => s.ExecuteAsync(state, default))
                .ReturnsAsync(true);

            while (await machine.MoveNextAsync()) { }

            stepMock.Verify(s => s.ExecuteAsync(state, default), Times.Exactly(steps.Count));
        }

        [Fact]
        public async Task MoveNextAsync_Should_ReturnFalse_When_FirstStepReturnsFalse()
        {
            var trueStepMock = new Mock<IStateMachineStep<DummyState>>();
            var falseStepMock = new Mock<IStateMachineStep<DummyState>>();

            var steps = new List<IStateMachineStep<DummyState>>
            {
                falseStepMock.Object,
                trueStepMock.Object,
                trueStepMock.Object,
                trueStepMock.Object
            };

            var state = new DummyState();
            var machine = new StateMachine<DummyState>(steps, state);

            trueStepMock
                .Setup(s => s.ExecuteAsync(state, default))
                .ReturnsAsync(true);

            Assert.False(await machine.MoveNextAsync());
        }

        [Fact]
        public async Task MoveNextAsync_Should_Interrupt_When_OneStepReturnsFalse()
        {
            var trueStepMock = new Mock<IStateMachineStep<DummyState>>();
            var falseStepMock = new Mock<IStateMachineStep<DummyState>>();

            var steps = new List<IStateMachineStep<DummyState>>
            {
                trueStepMock.Object,
                trueStepMock.Object,
                falseStepMock.Object,
                trueStepMock.Object
            };

            var state = new DummyState();
            var machine = new StateMachine<DummyState>(steps, state);

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

        public class DummyState
        {

        }
    }
}
