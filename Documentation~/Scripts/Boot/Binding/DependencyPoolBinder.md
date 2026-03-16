# [`DependencyPoolBinder`](/Scripts/Boot/Binding/DependencyPoolBinder.cs)

[`Binder`](/Documentation~/Scripts/Boot/Binding/Binder.md) for creating a [`DependencyPool`](/Documentation~/Scripts/Dependencies/DependencyPool.md).

Creates and initializes a pool of [`IDependency`](/Documentation~/Scripts/Dependencies/IDependency.md) for [`DependencyPool`](/Documentation~/Scripts/Dependencies/DependencyPool.md).

Determines the number of objects via the abstract property `_count`.

Unlike [`DependencyAbstractPoolBinder`](/Documentation~/Scripts/Boot/Binding/DependencyPoolBinder.md), it already includes an override for creating a model using `new T()`.

Used in [`BinderSystem`](/Documentation~/Scripts/Boot/Binding/BinderSystem.md) and [`ModelsContext`](/Documentation~/Scripts/Boot/Contexts/ModelsContext.md).