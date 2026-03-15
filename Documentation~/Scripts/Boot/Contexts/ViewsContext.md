# [`ViewsContext`](/Scripts/Boot/Contexts/ViewsContext.cs)

This is a container for the scene's [`View`](/Documentation~/Scripts/Views/View.md).

Stores components inside:
1. `_assets` - manually added components
2. `_generated` - for objects automatically found in the scene

In the Editor, it provides the Unity `Reset` method to automatically search for objects with the [`IGeneratedContext`](/Documentation~/Scripts/Views/Generated/IGeneratedContext.md) interface, sort them by [`IGeneratedPriority`](/Documentation~/Scripts/Views/Generated/IGeneratedPriority.md), and populate the `_generated` array.

Instances are created inside `Instantiate()` when the scene loads.

Generates the general lists `mainViews` and `subViews`, ensuring initialization:
1. [`Init`](/Documentation~/Scripts/Loop/IInit.md) and [`InitAsync`](/Documentation~/Scripts/Loop/IInitAsync.md)
2. [`BeginPlay`](/Documentation~/Scripts/Loop/IBeginPlay.md) and [`BeginPlayAsync`](/Documentation~/Scripts/Loop/IBeginPlayAsync.md)
3. [`IApplyResolving`](/Documentation~/Scripts/Dependencies/IApplyResolving.md)

It is possible to connect a [`View`](/Documentation~/Scripts/Views/View.md) during gameplay via `Connect` and disconnect it via `Disconnect`.

Deinitialization is performed via `IUnload` and disables all child controllers if their parent is disconnected.

There is a separate [`ViewsEmptyContext`](/Documentation~/Scripts/Boot/Empty/ViewsEmptyContext.md) class for use in scenes without [`View`](/Documentation~/Scripts/Views/View.md) creation logic.