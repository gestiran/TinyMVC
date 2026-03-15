# SceneContext.cs

The SceneContext is the central control component that initializes and binds all parts: Model, View, Controller, Parameters, and Binding.

Inherits from: `MonoBehaviour`, `IUnloadLink`

Initializes and manages the scene's lifecycle.
Creates controller, model, and parameter contexts via the abstract `Create` methods.
Registers components via the `components` array, connects ticks: `FixedTick`, `Tick`, `LateTick`.
Resolves dependencies via `DependencyContainer`, prepares and starts `View` and `Controller` asynchronously via `InitAsync`.
Supports cancellation via `CancellationToken` and unloading via `IUnloadLink`.