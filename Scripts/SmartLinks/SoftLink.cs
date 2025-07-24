// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using UnityEngine;

#if ODIN_INSPECTOR && UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.SmartLinks {
    [Serializable]
#if ODIN_INSPECTOR && UNITY_EDITOR
    [InlineProperty]
#endif
    public sealed class SoftLink<T> : SmartLink<T> where T : MonoBehaviour {
        [SerializeField]
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [HorizontalGroup, SuffixLabel("Root", true), HideLabel, Required]
    #endif
        private Transform _root;
        
        public T GetInstance() => GetInstance(_ => { });
        
        public T GetInstance(Action<T> initialization) => getInstance(_root, initialization);
    }
}