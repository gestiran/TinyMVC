# ILateTick.cs

A marker that defines a contract for executing logic during Unity's `LateUpdate` phase.

Inherits from: ILoop

Requires the implementing class to have a `LateTick()` method, which is called by the system every frame after all regular `ITick` updates have completed.