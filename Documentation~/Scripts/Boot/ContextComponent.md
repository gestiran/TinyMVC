# ContextComponent.cs

A modular extension of `SceneContext` for registering logic.

Inherits from: `MonoBehaviour`

Creates `View` prefabs from the serializable `_assets` array via `Instantiate`.
Delegates the creation of controllers, models, parameters, and `Binder`s to subclasses via the virtual methods `CreateControllers`, `CreateModels`, `CreateParameters`, and `CreateBinders`.
Registers `Controller` in the system’s shared lists via the passed `_systems` references.
Adds dependencies and bound models to shared data contexts via the `Load` and `AddBinder` methods, using `ProjectContext.data` for storage.
Called via `SceneContext` to build the dependency graph and initialize scene subsystems.