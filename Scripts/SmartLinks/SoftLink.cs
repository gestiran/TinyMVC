using System;
using UnityEngine;

#if ODIN_INSPECTOR && UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.SmartLinks {
#if ODIN_INSPECTOR && UNITY_EDITOR
    [InlineProperty]
#endif
    [Serializable]
    public sealed class SoftLink<T> : SmartLink<T> where T : MonoBehaviour {
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [HorizontalGroup, SuffixLabel("Root", true), HideLabel, ChildGameObjectsOnly(IncludeInactive = true), Required]
    #endif
        [SerializeField]
        private Transform _root;

        public T GetInstance() => GetInstance(_ => { });

        public T GetInstance(Action<T> initialization) => getInstance(_root, initialization);
    }
}