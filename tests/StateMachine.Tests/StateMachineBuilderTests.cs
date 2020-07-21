using System;
using System.Collections.Generic;
using System.Text;

using StateMachines.Exceptions;

using Xunit;

namespace StateMachines.Tests
{
    public class StateMachineBuilderTests
    {
        [Fact]
        public void Build_Should_Throw_When_NoStepIsProvided()
        {
            var builder = StateMachineBuilder.Create<DummyState, DummyData>()
                .SetEndState(DummyState.Done);

            Assert.Throws<MissingStepException>(() => builder.Build(DummyState.Start, new DummyData()));
        }

        [Fact]
        public void AddStep_Should_Throw_When_StepIsNull()
        {
            var builder = StateMachineBuilder.Create<DummyState, DummyData>();

            Assert.Throws<ArgumentNullException>(() => builder.AddStep((IStateMachineStep<DummyState, DummyData>)null));
        }

        [Fact]
        public void AddSteps_Should_Throw_When_StepIsNull()
        {
            var builder = StateMachineBuilder.Create<DummyState, DummyData>();

            Assert.Throws<ArgumentNullException>(() => builder.AddSteps((IEnumerable<IStateMachineStep<DummyState, DummyData>>)null));
        }

        enum DummyState
        {
            Start,
            Done
        }

        class DummyData { }
    }
}
