# IGlobalContext.cs

This is a marker that designates `SceneContext` as global.

It signals the scene context to call the `DontDestroyOnLoad` method on itself, preventing it from being destroyed when a new scene is loaded.
It preserves the availability of all registered controllers and models.