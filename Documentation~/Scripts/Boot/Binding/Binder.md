# Binder.cs

This is a factory responsible for creating and initializing `IDependency` models.

Inherits from: IBinder

Defines the contract for creating dependencies via the `GetDependency()` method, stores a unique `_key` for persistence systems, and maintains a reference to `UnloadPool` for cleaning up resources when the scene changes.
Implements a pattern for creating an instance of a model of type `T`, ensuring single-instance initialization.
Extension is possible via `BinderExtension`.
There is a separate `ActorBinder` for initializing `Actor` models with auto-binding to `View`.