# ControllersPropertyProcessor.cs

An extension for the Editor.

Inherits from `OdinValueDrawer<T>` from the Odin Inspector plugin.

Displays fields of type `IController` in the Inspector window.
Allows you to disable updates for `ILoop` types, such as `ITick`, `IFixedTick`, and `ILiteTick`, individually for each controller.