using System;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityObject = UnityEngine.Object;

namespace TinyMVC.SmartLinks {
    [Serializable, InlineProperty]
    public abstract class SmartLink<T> where T : MonoBehaviour {
        public bool isCreated { get; private set; }
        
        [SerializeField, HorizontalGroup, SuffixLabel("Prefab", true), DisableIf(nameof(isCreated)), HideLabel, Required]
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
        
    #if UNITY_EDITOR
        public T GetPrefab() => _prefab;
        
    #endif
    }
}