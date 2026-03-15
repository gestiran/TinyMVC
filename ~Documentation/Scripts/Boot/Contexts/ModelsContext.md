# ModelsContext.cs

Serves as a container for managing the lifecycle of `IDependency` models.

Registers models using `Binder`, `BinderSystem`, or `BinderMod`, automatically checking binding conditions via `IBindConditions`.
Connects objects to the `UnloadPool`.
Stores created dependencies in the `dependenciesBinded` and `dependencies` lists, adding them to `ProjectContext.data` by scene key.

Splits the process into two stages:
1. `Bind`
2. `Create`

Provides a nested `EmptyContext` for scenes without models.
Performs resource cleanup via `IUnload`, triggering the unloading of all dependencies.