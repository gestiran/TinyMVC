# IInitAsync.cs

This is an interface that defines a contract for the early asynchronous initialization of components.

Requires the implementing class to have an `IInitAsync()` method, which is called by the system immediately after the scene is loaded, but before the dependency initialization phase.
Intended for configuring internal `View` parameters and preparing the object's state before receiving `IDependency` dependencies.
The basic non-asynchronous implementation is contained in `IInit`.