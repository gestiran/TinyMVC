# ViewGlobalPool.cs

An abstract pool that extends the functionality of `View` to manage a collection of objects.

Inherits from: `View`

Contains an array of references to all `View` objects in the scene that match the specified type.
Implements the `IEnumerable<View>` interface for iterating over the collection in `foreach` loops.
Provides an indexer for direct access to elements by index and a `GetTypeView` method to retrieve the type of stored views.
In `Editor`, it automatically detects and populates the array with found `View` objects when `UpdateViews` is called or when the component’s `Reset` is called.