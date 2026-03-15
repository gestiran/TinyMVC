# [`ActorBinder`](/Scripts/Boot/Binding/ActorBinder.cs)

These are specialized factories for creating [`Actor`](/Documentation~/Scripts/Dependencies/Components/Actor.md) type models, automatically binding them to a specific [`View`](/Documentation~/Scripts/Views/View.md).

Inherits from: Binder<TActor> where TActor : [`Actor`](/Documentation~/Scripts/Dependencies/Components/Actor.md)

Automatically attaches a reference to the [`View`](/Documentation~/Scripts/Views/View.md) to the instance being created.

Class constructors allow you to pass the target [`View`](/Documentation~/Scripts/Views/View.md) explicitly or use `null` for subsequent assignment, as well as specify a unique model key for save systems.

Used in scenarios where an [`Actor`](/Documentation~/Scripts/Dependencies/Components/Actor.md) is inextricably linked to a [`View`](/Documentation~/Scripts/Views/View.md) and cannot exist independently.

Extension is possible via [`BinderExtension`](/Documentation~/Scripts/Boot/Binding/BinderExtension.md).