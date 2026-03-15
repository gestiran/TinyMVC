# IDontDestroyOnLoad.cs

This is a marker indicating that an object must be preserved when new scenes are loaded.

It serves as an indicator for the `SceneContext` initialization system to apply the `Object.DontDestroyOnLoad` method to the Unity object, preventing it from being destroyed when transitioning between Unity scenes.
It is used exclusively in the context of `IGlobalContext` global scenes.