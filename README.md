# StateMachines

A simple, linear state machine, for simple, linear processes.

## How it works

The main type is the interface `IStateMachine<int, TData>`.

```csharp
public interface IStateMachine<TState, TData> where TState : struct
{
    TState State { get; }
    TData Data { get; }
    Task<bool> MoveNextAsync(CancellationToken cancellationToken = default);
}
```

But you'll not have to implemente it.

A StateMachine is made of steps. Each time you call `MoveNextAsync` and it returns true, the machine moves to the next step.

And it's the steps that you'll have to implement.

Steps are defined by the interfaces `IStateMachineStep<TState, TData>` and `IStateMachineStep<TData>`. 

They look like this:

```csharp
public interface IStateMachineStep<TData>
{
    Task<bool> ExecuteAsync(TData data, CancellationToken cancellationToken = default);
}

public interface IStateMachineStep<TState, TData> : IStateMachineStep<TData>
    where TState : struct
{
    TState State { get; }
}
```

You build the StateMachine with a builder, like the folowing code.

```csharp
var machine = StateMachineBuilder.Create<DummyData>()
      .AddStep(new CounterStep1())
      .AddStep(new CounterStep2())
      .AddStep(new CounterStep3())
      .Build(dummy);

// moves until the end or one step returns false.
while (await machine.MoveNextAsync())
{
    //...
}
```

The builder can builde two types of state machine, int state StateMachines, and general struc state machine (usually used for enums).

The last example usend a int state StateMachine.

With the int state StateMachine, your steps need to implement the `IStateMachineStep<TData>` interface.

For example:
```csharp
public class CounterStep1 : IStateMachineStep<DummyData>
{
    public Task<bool> ExecuteAsync(DummyData data, CancellationToken cancellationToken = default)
    {
        data.StepCount += 1;
        return Task.FromResult(true);
    }
}
```
