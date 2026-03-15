# [`ControllersPropertyProcessor<T>`](/Editor/ControllersPropertyProcessor.cs)

An extension for the Editor.

Inherits from `OdinValueDrawer<T>` from the [OdinInspector](https://odininspector.com/) plugin.

Displays fields of type [`IController`](/Documentation~/Scripts/Controllers/Controller.md) in the Unity Inspector window.
Allows you to disable updates for [`ILoop`](/Documentation~/Scripts/Loop/ILoop.md) types, such as [`ITick`](/Documentation~/Scripts/Loop/ITick.md), [`IFixedTick`](/Documentation~/Scripts/Loop/IFixedTick.md), and [`ILiteTick`](/Documentation~/Scripts/Loop/ILateTick.md), individually for each controller.