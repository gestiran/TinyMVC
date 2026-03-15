# [`IGlobalContext`](/Scripts/Boot/Contexts/IGlobalContext.cs)

This is a marker that designates [`SceneContext`](/Documentation~/Scripts/Boot/SceneContext.md) as global.

It signals the scene context to call the Unity `DontDestroyOnLoad` method on itself, preventing it from being destroyed when a new scene is loaded.

It preserves the availability of all registered controllers and models.