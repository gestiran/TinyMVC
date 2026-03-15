# IBeginPlayAsync.cs

This is a marker that defines the contract for executing the game startup logic at the end of the initialization phase.

It requires the implementing class to have a `BeginPlay()` method, which is called asynchronously by the system after all `View` and `Controller` instances have completed the `Init` and `ApplyResolve` initialization steps.
The basic non-asynchronous implementation is contained in `IBeginPlay`.