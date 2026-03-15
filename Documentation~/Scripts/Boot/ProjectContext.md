# [`ProjectContext`](/Scripts/Boot/ProjectContext.cs)

Context and scene lifecycle manager.

Manages the global project state ([`ProjectComponents`](/Documentation~/Scripts/Boot/ProjectComponents.md), [`ProjectData`](/Documentation~/Scripts/Boot/ProjectData.md)) and active [`SceneContext`](/Documentation~/Scripts/Boot/SceneContext.md) instances.

Initializes automatically via the Unity `RuntimeInitializeOnLoadMethod` attribute.

Provides asynchronous methods for loading and unloading scenes `LoadScene`, `AddScene`, `RemoveScene` via `UnityEngine.SceneManagement`.

Implements scene reload logic by temporarily loading an empty scene to ensure proper memory release.

Maintains [`SceneContext`](/Documentation~/Scripts/Boot/SceneContext.md) entries in the `_contexts` and `_sceneContexts` dictionaries.

Automatically calls the `Create`, `InitAsync`, `Unload`, and `Remove` methods on attached [`SceneContext`](/Documentation~/Scripts/Boot/SceneContext.md) instances when the scene changes.