# [`ContextLink`](/Scripts/Boot/Helpers/ContextLink.cs)

This is a wrapper for securely storing a reference to a context along with its unique identification key.

Encapsulates a pair consisting of a string key and a `T` value.

Comparison logic is implemented by overriding `Equals` and `GetHashCode`, based solely on the key.

Used within [`ProjectContext`](/Documentation~/Scripts/Boot/ProjectContext.md) to manage active contexts.

Functionality extensions are contained in [`ContextExtensions`](/Documentation~/Scripts/Boot/Helpers/ContextExtensions.md).