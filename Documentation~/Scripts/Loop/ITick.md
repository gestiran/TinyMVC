# ITick.cs

This is an interface that defines the contract for executing logic within Unity's main `Update` loop.

Inherits from: ILoop

Requires the implementing class to have a `Tick()` method, which is called by the system every frame during the standard `Update` cycle.
Used by `View` and `Controller` components that need to respond to changes in the game state at the frame rendering rate.