# [`ILoop`](/Scripts/Loop/ILoop.cs)

This is a marker that unifies all component types involved in the update loop.

It serves as a common type for the [`IFixedTick`](/Documentation~/Scripts/Loop/IFixedTick.md), [`ITick`](/Documentation~/Scripts/Loop/ITick.md), and [`ILateTick`](/Documentation~/Scripts/Loop/ILateTick.md) interfaces, allowing the [`SceneContext`](/Scripts/Boot/SceneContext.cs) scene management system

It contains no methods of its own, serving as an architectural contract for identifying objects that require periodic logic execution.

It enables the extension of the tick system without changing the signatures of hook methods, allowing new types of updates to be added by inheriting from `ILoop`.