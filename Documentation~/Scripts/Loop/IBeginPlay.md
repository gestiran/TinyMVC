# IBeginPlay.cs

This is a marker that defines a contract for executing the game startup logic at the end of the initialization phase.

It requires the implementing class to have a `BeginPlay()` method, which is called by the system after all `View` and `Controller` instances have completed the `Init` and `ApplyResolve` initialization steps.
The asynchronous version is implemented in `IBeginPlayAsync`.