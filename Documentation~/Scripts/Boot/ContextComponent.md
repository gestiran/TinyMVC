# [`ContextComponent`](/Scripts/Boot/ContextComponent.cs)

A modular extension of [`SceneContext`](/Documentation~/Scripts/Boot/SceneContext.md) for registering logic.

Inherits from: `MonoBehaviour`

Creates [`View`](/Documentation~/Scripts/Views/View.md) prefabs from the serializable `_assets` array via `Instantiate`.

Delegates the creation of controllers, models, parameters, and [`Binder`](/Documentation~/Scripts/Boot/Binding/Binder.md) to subclasses via the virtual methods `CreateControllers`, `CreateModels`, `CreateParameters`, and `CreateBinders`.

Registers [`Controller`](/Documentation~/Scripts/Controllers/IController.md) in the system’s shared lists via the passed `_systems` references.

Adds dependencies and bound models to shared data contexts via the `Load` and `AddBinder` methods, using [`ProjectContext`](/Documentation~/Scripts/Boot/ProjectContext.md).[`data`](/Documentation~/Scripts/Boot/ProjectData.md) for storage.

Called via [`SceneContext`](/Documentation~/Scripts/Boot/SceneContext.md) to build the dependency graph and initialize scene subsystems.