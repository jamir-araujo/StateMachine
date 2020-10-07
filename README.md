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

The builder can builde two types of state machine, int state StateMachines, and general struct StateMachine (usually for enums).

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

With the struct StateMachine, you can use any stuct as the state for the StateMachine.

For example:
```csharp
public enum ProcessState { Start, Step1, Done }

public class StartStep : IStateMachineStep<ProcessState, DummyData>
{
    public ProcessState State => ProcessState.Start;

    public Task<bool> ExecuteAsync(DummyData data, CancellationToken cancellationToken = default)
    {
        data.StepCount += 1;
        return Task.FromResult(true);
    }
}
```

End you build like this:

```csharp
var machine = StateMachineBuilder.Create<ProcessState, DummyData>()
      .AddStep(new StartStep())
      .AddStep(new Step1())
      .SetEndState(ProcessState.Done)
      .Build(ProcessState.Start, dummy);

// moves until the end or one step returns false.
while (await machine.MoveNextAsync())
{
    //...
}
```

Note the `SetEndState` method. When you are using this type os StateMachines, you need to inform which is the end state.


## Using with ASP.NET Core

Configure a StateMachine:

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddStateMachine<DummyData>(stateMachine =>
        {
            stateMachine
                .AddStep<CounterStep1>()
                .AddStep<CounterStep2>()
                .AddStep<CounterStep3>()
                .AddStep<CounterStep4>();
        });
    }
}
```

Then request it with the `IStateMachineFactory`:
```csharp
public class MyService
{
    //...
    public MyService(IStateMachineFactory factory)
    {
        _factory = factory;
    }
    
    public async Task ExecuteProcessAsync(DymmyData Data)
    {
        var machine = _factory.Create(data);
        
        while (await machine.MoveNextAsync())
        {
            //...
        }
    }
}
```

