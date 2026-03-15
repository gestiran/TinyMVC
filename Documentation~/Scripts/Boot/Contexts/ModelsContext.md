# [`ModelsContext`](/Scripts/Boot/Contexts/ModelsContext.cs)

Serves as a container for managing the lifecycle of [`IDependency`](/Documentation~/Scripts/Dependencies/IDependency.md) models.

Registers models using [`Binder`](/Documentation~/Scripts/Boot/Binding/Binder.md), [`BinderSystem`](/Documentation~/Scripts/Boot/Binding/BinderSystem.md), or [`BinderMod`](/Documentation~/Scripts/Boot/Binding/BinderMod.md), automatically checking binding conditions via [`IBindConditions`](/Documentation~/Scripts/Boot/Binding/IBindConditions.md).

Connects objects to the `UnloadPool`.

Stores created dependencies in the `dependenciesBinded` and `dependencies` lists, adding them to [`ProjectContext`](/Documentation~/Scripts/Boot/ProjectContext.md).[`data`](/Documentation~/Scripts/Boot/ProjectData.md) by scene key.

Splits the process into two stages:
1. `Bind`
2. `Create`

Provides a nested `EmptyContext` for scenes without models.

Performs resource cleanup via `IUnload`, triggering the unloading of all dependencies.