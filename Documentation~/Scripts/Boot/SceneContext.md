# [`SceneContext`](/Scripts/Boot/SceneContext.cs)

The scene context is the central control component that initializes and binds all parts:
- [`Model`](/Documentation~/Scripts/Dependencies/IDependency.md)
- [`View`](/Documentation~/Scripts/Views/View.md)
- [`Controller`](/Documentation~/Scripts/Controllers/IController.md)
- [`Parameters`](/Documentation~/Scripts/Dependencies/IDependency.md)
- [`Binding`](/Documentation~/Scripts/Boot/Binding/Binder.md)

Inherits from Unity `MonoBehaviour`, `IUnloadLink`

Initializes and manages the scene's lifecycle.

Creates controller, model, and parameter contexts via the abstract `Create` methods.

Registers components via the `components` array, connects ticks:
- [`FixedTick`](/Documentation~/Scripts/Loop/IFixedTick.md)
- [`Tick`](/Documentation~/Scripts/Loop/ITick.md)
- [`LateTick`](/Documentation~/Scripts/Loop/ILateTick.md)

Resolves dependencies via [`DependencyContainer`](/Documentation~/Scripts/Dependencies/DependencyContainer.md), prepares and starts [`View`](/Documentation~/Scripts/Views/View.md) and [`Controller`](/Documentation~/Scripts/Controllers/IController.md) asynchronously via `InitAsync`.

Supports cancellation via `CancellationToken` and unloading via `IUnloadLink`.