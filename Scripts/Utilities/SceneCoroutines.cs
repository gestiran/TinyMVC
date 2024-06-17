using System.Collections;
using UnityEngine;

namespace TinyMVC.Utilities {
    [DisallowMultipleComponent]
    public sealed class SceneCoroutines : MonoBehaviour {
        internal Coroutine AddCoroutine(IEnumerator enumerator) => StartCoroutine(enumerator);
        
        internal void RemoveCoroutine(Coroutine coroutine) => StopCoroutine(coroutine);
        
        internal void StopAll() => StopAllCoroutines();
    }
}