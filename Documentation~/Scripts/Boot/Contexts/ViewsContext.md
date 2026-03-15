# ViewsContext.cs

This is a container for the scene's `View`.

Stores components inside:
1. `_assets` - manually added components
2. `_generated` - for objects automatically found in the scene

In the Editor, it provides the `Reset` method to automatically search for objects with the `IGeneratedContext` interface, sort them by `IGeneratedPriority`, and populate the `_generated` array.

Instances are created inside `Instantiate()` when the scene loads.

Generates the general lists `mainViews` and `subViews`, ensuring initialization:
1. `Init` and `InitAsync`
2. `BeginPlay` and `BeginPlayAsync`
3. `IApplyResolving`

It is possible to connect a `View` during gameplay via `Connect` and disconnect it via `Disconnect`.

Deinitialization is performed via `IUnload` and disables all child controllers if their parent is disconnected.

There is a separate `ViewsEmptyContext` class for use in scenes without `View` creation logic.