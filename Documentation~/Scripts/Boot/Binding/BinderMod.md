# [`BinderMod`](/Scripts/Boot/Binding/BinderMod.cs)

An abstract [`Binder`](/Documentation~/Scripts/Boot/Binding/Binder.md) for binding existing [`IDependency`](/Documentation~/Scripts/Dependencies/IDependency.md) models.

Provides a mechanism for modifying a previously created model based on new data from the current context.

Obtains the model via [`ProjectContext`](/Documentation~/Scripts/Boot/ProjectContext.md).[`data`](/Documentation~/Scripts/Boot/ProjectData.md).`Get(out T model)`.

Used in [`BinderSystem`](/Documentation~/Scripts/Boot/Binding/BinderSystem.md) and [`ModelsContext`](/Documentation~/Scripts/Boot/Contexts/ModelsContext.md).