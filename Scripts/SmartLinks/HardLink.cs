using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace TinyMVC.SmartLinks {
    [Serializable, InlineProperty]
    public sealed class HardLink<T> : SmartLink<T> where T : MonoBehaviour {
        public T GetInstance(Transform parent) => GetInstance(parent, _ => { });
        
        public T GetInstance(Transform parent, Action<T> initialization) => getInstance(parent, initialization);
    }
}