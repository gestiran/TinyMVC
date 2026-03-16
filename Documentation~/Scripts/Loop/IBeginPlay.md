# [`IBeginPlay`](/Scripts/Loop/IBeginPlay.cs)

This is a marker that defines a contract for executing the game startup logic at the end of the initialization phase.

It requires the implementing class to have a `BeginPlay()` method, which is called by the system after all [`View`](/Documentation~/Scripts/Views/View.md) and [`Controller`](/Documentation~/Scripts/Controllers/IController.md) instances have completed the [`Init`](/Documentation~/Scripts/Loop/IInit.md) and [`ApplyResolve`](/Documentation~/Scripts/Dependencies/IApplyResolving.md) initialization steps.

The asynchronous version is implemented in [`IBeginPlayAsync`](/Documentation~/Scripts/Loop/IBeginPlayAsync.md).