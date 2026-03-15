# IUpdateConnection.cs

This is an interface that defines a contract for dynamically updating the state of child `View`s without fully re-rendering them.

It requires the implementing class to have an `UpdateConnection()` method, which is called by the parent `View` via the `UpdateConnections` method.