# [`ProjectData`](/Scripts/Boot/ProjectData.cs)

Project data container.

Stores and manages [`IDependency`](/Scripts/Dependencies/IDependency.cs) dependencies in contexts.

Uses `DependencyContainer` to store dependencies by type.

Supports adding and removing dependencies to the global context and to individual named contexts.

Provides dependency lookup by type via `TryGetDependency`, `Get`, `GetReference`, and `ForEachReference`.

Dependency retrieval priority:
1. `tempContainer`
2. `viewsContainer`
3. Other contexts

When adding a new context, initializes the associated dictionary `_components.all[contextKey]`.

In the Unity Editor, triggers the `onAdd` and `onRemove` events.