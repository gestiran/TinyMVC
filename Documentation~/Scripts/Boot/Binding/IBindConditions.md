# [`IBindConditions`](/Scripts/Boot/Binding/IBindConditions.cs)

A marker that defines the condition for performing dependency binding.

Used by the [`Binder`](/Documentation~/Scripts/Boot/Binding/Binder.md) and [`BinderMod`](/Documentation~/Scripts/Boot/Binding/BinderMod.md) classes to check whether binding is required.

If `IsNeedBinding()` returns `false`, the binding is skipped.