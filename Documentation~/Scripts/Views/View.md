# View.cs

A base class for MVC visual components that manages their lifecycle, hierarchy, and connections to the scene context.

Inherits from: MonoBehaviour

Implements mechanisms for connecting and disconnecting views (`Connect` and `Disconnect`) to `SceneContext`, automatically setting the `root` and `connectState` properties.
Allows passing temporary dependencies (`IDependency`, `DependencyContainer`) at connection time via `ProjectContext.data.tempContainer` for injection into new instances.
Supports type filtering to manage groups of related `View`s via the `DisconnectAll<T>` and `UpdateConnections` methods.
Provides `Reconnect` with automatic pre-disconnection.
Interacts with the `SceneContext`, `ProjectContext`, and `IDependency` classes, as well as the `IUpdateConnection` interface, to update connections.
Contains an internal `_connections` list for tracking child elements and preventing re-initialization via `InitSingle`.
Has a set of extensions within `ViewExtension`.