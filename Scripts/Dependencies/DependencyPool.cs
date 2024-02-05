using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#if ODIN_INSPECTOR && UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Dependencies {
#if ODIN_INSPECTOR && UNITY_EDITOR
    [HideLabel, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class DependencyPool<T> : IDependency where T : IDependency {
        public int length => _objects.Length;

    #if ODIN_INSPECTOR && UNITY_EDITOR
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, IsReadOnly = true)]
        [ShowInInspector, HideInEditorMode, HideReferenceObjectPicker, HideDuplicateReferenceBox, LabelText("@ToString()")]
    #endif
        private readonly T[] _objects;
        
        public DependencyPool(int count) => _objects = new T[count];

        public DependencyPool([NotNull] params DependencyPool<T>[] pools) {
            int count = 0;

            for (int poolId = 0; poolId < pools.Length; poolId++) {
                count += pools[poolId].length;
            }

            _objects = new T[count];
            count = 0;
            
            for (int poolId = 0; poolId < pools.Length; poolId++) {
                Array.Copy(pools[poolId]._objects, 0, _objects, count, pools[poolId].length);
                count += pools[poolId].length;
            }
        }

        public DependencyPool(List<T> pool) => _objects = pool.ToArray();
        
        private DependencyPool(DependencyPool<T> pool) { } // Nothing

        public T this[int index] {
            get => _objects[index];
            set => _objects[index] = value;
        }

        public override string ToString() => $"{GetType().Name}<{typeof(T).Name}>";
    }
}