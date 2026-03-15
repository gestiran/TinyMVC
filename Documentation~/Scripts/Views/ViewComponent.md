# ViewComponent.cs

Serves as a marker for components that are part of the `View`.

Inherits from: MonoBehaviour

Acts as a unified type for identifying and grouping logical parts of the View through the inheritance system, without implementing its own game logic at runtime.
Provides the `Reset` method to the `Editor` for forcing an object to be marked as modified via `EditorUtility.SetDirty`.
Used as a foundation for creating specific behavior scripts that must explicitly refer to `View`.