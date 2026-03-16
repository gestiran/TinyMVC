# [`ObservedDependencyListBinder`](/Scripts/Boot/Binding/ObservedDependencyListBinder.cs)

[`Binder`](/Documentation~/Scripts/Boot/Binding/Binder.md) to create an [`ObservedDependencyList`](/Documentation~/Scripts/Dependencies/ObservedDependencyList.md).

Creates and initializes an instance of [`ObservedDependencyList`](/Documentation~/Scripts/Dependencies/ObservedDependencyList.md).

Initializes the `_capacity` list, then calls `Bind(ObservedDependencyList<T>)` to populate it.

Uses the abstract method `Bind(ObservedDependencyList<T> model)` to configure the list items.

Provides the helper method `AddCount` for adding multiple items.