# ProjectContext.cs

Context and scene lifecycle manager.

Manages the global project state (`ProjectComponents`, `ProjectData`) and active `SceneContext` instances.
Initializes automatically via the `RuntimeInitializeOnLoadMethod` attribute.
Provides asynchronous methods for loading and unloading scenes `LoadScene`, `AddScene`, `RemoveScene` via `UnityEngine.SceneManagement`.
Implements scene reload logic by temporarily loading an empty scene to ensure proper memory release.
Maintains `SceneContext` entries in the `_contexts` and `_sceneContexts` dictionaries.
Automatically calls the `Create`, `InitAsync`, `Unload`, and `Remove` methods on attached `SceneContext` instances when the scene changes.