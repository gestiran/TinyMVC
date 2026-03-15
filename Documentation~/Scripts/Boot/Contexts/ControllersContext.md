# [`ControllersContext`](/Scripts/Boot/Contexts/ControllersContext.cs)

This is a container that manages the lifecycle, registration, and interaction of all [`IController`](/Documentation~/Scripts/Controllers/Controller.md) instances.

Inherits from: [`IController`](/Documentation~/Scripts/Controllers/Controller.md)

Contains controllers in two structures:
1. `systems` - for initialization at the start of a scene within [`SceneContext`](/Documentation~/Scripts/Boot/SceneContext.md)
2. `_controllers` - for controllers connected during gameplay via `Connect` and disconnected via `Disconnect`

Implements full initialization:
1. [`Init`](/Documentation~/Scripts/Loop/IInit.md) and [`InitAsync`](/Documentation~/Scripts/Loop/IInitAsync.md) - initialization
2. [`IApplyResolving`](/Documentation~/Scripts/Dependencies/IApplyResolving.md) - resolving dependencies
3. [`BeginPlay`](/Documentation~/Scripts/Loop/IBeginPlay.md) and [`BeginPlayAsync`](/Documentation~/Scripts/Loop/IBeginPlayAsync.md) - initialization and logic execution

Deinitialization is performed via `IUnload` and disables all child controllers if their parent is disconnected.

Contains a nested class `EmptyContext` for use in scenes without controller logic.