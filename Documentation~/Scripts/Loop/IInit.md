# IInit.cs

This is a marker interface that defines a contract for the early initialization of components.

It requires the implementing class to have an `Init()` method, which is called by the system immediately after the scene is loaded, but before the dependency initialization phase.
It is intended for configuring the internal parameters of the `View` and preparing the object's state before receiving `IDependency` dependencies.
The asynchronous version is implemented in `IInitAsync`.