# ViewPool.cs

A container that extends `View` to manage a collection of child `View` objects.

Inherits from: View

Contains an array of references to all child `View` objects of the appropriate type.
Implements the `IEnumerable<View>` interface to allow the collection to be iterated over in `foreach` loops.
Provides an indexer for direct access to elements by index and a `GetTypeView` method to retrieve the type of stored views.
In `Editor`, it automatically detects and populates the array with found `View` objects when `UpdateViews` is called or when the component’s `Reset` is called.