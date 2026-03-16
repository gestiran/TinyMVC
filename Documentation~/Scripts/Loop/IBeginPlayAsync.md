# [`IBeginPlayAsync`](/Scripts/Loop/IBeginPlayAsync.cs)

This is a marker that defines the contract for executing the game startup logic at the end of the initialization phase.

It requires the implementing class to have a `BeginPlay()` method, which is called asynchronously by the system after all [`View`](/Documentation~/Scripts/Views/View.md) and [`Controller`](/Documentation~/Scripts/Controllers/IController.md) instances have completed the [`Init`](/Documentation~/Scripts/Loop/IInit.md) and [`ApplyResolve`](/Documentation~/Scripts/Dependencies/IApplyResolving.md) initialization steps.

The basic non-asynchronous implementation is contained in [`IBeginPlay`](/Documentation~/Scripts/Loop/IBeginPlay.md).