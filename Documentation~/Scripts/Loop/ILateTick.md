# [`ILateTick`](/Scripts/Loop/ILateTick.cs)

A marker that defines a contract for executing logic during Unity's `LateUpdate` phase.

Inherits from [`ILoop`](/Documentation~/Scripts/Loop/ILoop.md).

Requires the implementing class to have a `LateTick()` method, which is called by the system every frame after all regular [`ITick`](/Documentation~/Scripts/Loop/ITick.md) updates have completed.