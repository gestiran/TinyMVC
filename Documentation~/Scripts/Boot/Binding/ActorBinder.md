# ActorBinder.cs

These are specialized factories for creating `Actor`-type models, automatically binding them to a specific `View`.

Inherits from: Binder<TActor> where TActor : Actor

Automatically attaches a reference to the `View` to the instance being created.
Class constructors allow you to pass the target `View` explicitly or use `null` for subsequent assignment, as well as specify a unique model key for save systems.
Used in scenarios where an `Actor` is inextricably linked to a `View` and cannot exist independently.
Extension is possible via `BinderExtension`.