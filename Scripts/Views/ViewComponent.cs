// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using UnityEngine;

namespace TinyMVC.Views {
    public abstract class ViewComponent : MonoBehaviour {
    #if UNITY_EDITOR
        
        public virtual void Reset() => UnityEditor.EditorUtility.SetDirty(this);
        
    #endif
    }
}