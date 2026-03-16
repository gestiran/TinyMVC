# [`IBinder`](/Scripts/Boot/Binding/IBinder.cs)

A generic interface for dependency binding classes.

Defines a contract for factories that create and prepare [`IDependency`](/Documentation~/Scripts/Dependencies/IDependency.md) for use.

The `GetDependency()` method creates and initializes a dependency instance, returning a ready-to-use [`IDependency`](/Documentation~/Scripts/Dependencies/IDependency.md) object.

The `current` property provides a reference to the [`Binder`](/Documentation~/Scripts/Boot/Binding/Binder.md) instance itself.