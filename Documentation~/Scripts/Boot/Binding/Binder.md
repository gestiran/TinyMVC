# [`Binder`](/Scripts/Boot/Binding/Binder.cs)

This is a factory responsible for creating and initializing [`IDependency`](/Documentation~/Scripts/Dependencies/IDependency.md) models.

Inherits from: [`IBinder`](/Documentation~/Scripts/Boot/Binding/IBinder.md)

Defines the contract for creating dependencies via the `GetDependency()` method, stores a unique `_key` for persistence systems, and maintains a reference to `UnloadPool` for cleaning up resources when the scene changes.

Implements a pattern for creating an instance of a model of type `T`, ensuring single-instance initialization.

Extension is possible via [`BinderExtension`](/Documentation~/Scripts/Boot/Binding/BinderExtension.md).

There is a separate [`ActorBinder`](/Documentation~/Scripts/Boot/Binding/ActorBinder.md) for initializing [`Actor`](/Documentation~/Scripts/Dependencies/Components/Actor.md) models with auto-binding to [`View`](/Documentation~/Scripts/Views/View.md).