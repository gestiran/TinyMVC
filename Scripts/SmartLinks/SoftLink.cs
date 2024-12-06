using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace TinyMVC.SmartLinks {
    [Serializable, InlineProperty]
    public sealed class SoftLink<T> : SmartLink<T> where T : MonoBehaviour {
        [SerializeField, HorizontalGroup, SuffixLabel("Root", true), HideLabel, ChildGameObjectsOnly(IncludeInactive = true), Required]
        private Transform _root;
        
        public T GetInstance() => GetInstance(_ => { });
        
        public T GetInstance(Action<T> initialization) => getInstance(_root, initialization);
    }
}