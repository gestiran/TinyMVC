# ControllersContext.cs

This is a container that manages the lifecycle, registration, and interaction of all `IController` instances.

Inherits from: IController

Contains controllers in two structures:
1. `systems` - for initialization at the start of a scene within `SceneContext`
2. `_controllers` - for controllers connected during gameplay via `Connect` and disconnected via `Disconnect`

Implements full initialization:
1. `Init` and `InitAsync` - initialization
2. `IApplyResolving` - resolving dependencies
3. `BeginPlay` and `BeginPlayAsync` - initialization and logic execution

Deinitialization is performed via `IUnload` and disables all child controllers if their parent is disconnected.

Contains a nested class `EmptyContext` for use in scenes without controller logic.