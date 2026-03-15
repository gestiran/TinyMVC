# ParametersContext.cs

Serves as a container for managing scene parameters, typically loaded as `ScriptableObject`s with the `IDependency` interface.

Dependencies are stored in the `all` list and added via the `Add<T>` method.
Exports the assembled list to external contexts via `AddDependencies`.
Contains a nested `EmptyContext` class for scenes that do not require external parameters.
Designed to store configuration data and resource references that must be available immediately after the scene loads, before game logic begins.