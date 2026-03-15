# IFixedTick.cs

This is an interface that defines the contract for executing logic within Unity's `FixedUpdate` loop.

Inherits from: ILoop

Requires the implementing class to have a `FixedTick()` method, which is called by the system at a fixed time step independent of the rendering frame rate.
Intended primarily for use with the physics engine, ensuring the stability and determinism of the simulation.