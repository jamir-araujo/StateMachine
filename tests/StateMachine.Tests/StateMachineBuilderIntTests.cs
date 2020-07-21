
using System;
using System.Collections.Generic;

using StateMachines.Exceptions;

using Xunit;

namespace StateMachines.Tests
{
    public class StateMachineBuilderIntTests
    {
        [Fact]
        public void Build_Should_Throw_When_NoStepIsProvided()
        {
            var builder = StateMachineBuilder.Create<DummyData>();

            Assert.Throws<MissingStepException>(() => builder.Build(new DummyData()));
        }

        [Fact]
        public void AddStep_Should_Throw_When_StepIsNull()
        {
            var builder = StateMachineBuilder.Create<DummyData>();

            Assert.Throws<ArgumentNullException>(() => builder.AddStep((IStateMachineStep<DummyData>)null));
        }

        [Fact]
        public void AddSteps_Should_Throw_When_StepIsNull()
        {
            var builder = StateMachineBuilder.Create<DummyData>();

            Assert.Throws<ArgumentNullException>(() => builder.AddSteps((IEnumerable<IStateMachineStep<DummyData>>)null));
        }

        enum DummyState
        {
            Start,
            Done
        }

        class DummyData { }
    }
}
