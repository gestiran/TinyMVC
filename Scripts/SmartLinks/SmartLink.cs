using System;
using UnityEngine;

#if ODIN_INSPECTOR && UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

using UnityObject = UnityEngine.Object;

namespace TinyMVC.SmartLinks {
    #if ODIN_INSPECTOR && UNITY_EDITOR
    [InlineProperty]
    #endif
    [Serializable]
    public abstract class SmartLink<T> where T : MonoBehaviour {
        public bool isCreated { get; private set; }
        
        #if ODIN_INSPECTOR && UNITY_EDITOR
        [HorizontalGroup, SuffixLabel("Prefab", true), HideLabel, DisableIf(nameof(isCreated)), Required]
        #endif
        [SerializeField]
        private T _prefab;
        
        protected Func<Transform, Action<T>, T> getInstance;
        
        protected SmartLink() => getInstance = Initialize;
        
        public bool TryGetInstance(out T instance) {
            instance = _prefab;
            
            return isCreated;
        }
        
        private T Initialize(Transform parent, Action<T> initialization) {
            _prefab = CreateInstance(_prefab, parent);
            initialization(_prefab);
            isCreated = true;
            getInstance = (_, _) => _prefab;
            
            return _prefab;
        }
        
        private T CreateInstance(T prefab, Transform root) => UnityObject.Instantiate(prefab, root);
    }
}