# [`IInit`](/Scripts/Loop/IInit.cs)

This is a marker interface that defines a contract for the early initialization of components.

It requires the implementing class to have an `Init()` method, which is called by the system immediately after the scene is loaded, but before the dependency initialization phase.

It is intended for configuring the internal parameters of the [`View`](/Documentation~/Scripts/Views/View.md) and preparing the object's state before receiving [`IDependency`](/Documentation~/Scripts/Dependencies/IDependency.md) dependencies.

The asynchronous version is implemented in [`IInitAsync`](/Documentation~/Scripts/Loop/IInitAsync.md).