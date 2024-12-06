using System;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

namespace TinyMVC.Boot.Helpers {
    internal abstract class ContextLink<T> : IEquatable<ContextLink<T>> {
        [HideInEditorMode, HideInPlayMode]
        public readonly int sceneId;
        
    #if UNITY_EDITOR
        private string _label;
    #endif
        
        [ShowInInspector, Title("@_label"), InlineProperty, HideLabel, HideInEditorMode, HideReferenceObjectPicker, HideDuplicateReferenceBox]
        public readonly T context;
        
        protected ContextLink(int sceneId, T context) {
            this.sceneId = sceneId;
            this.context = context;
        #if UNITY_EDITOR
            if (sceneId >= 0) {
                _label = SceneManager.GetSceneByBuildIndex(sceneId).name;
            } else {
                _label = SceneManager.GetActiveScene().name;
            }
        #endif
        }
        
        public bool Equals(ContextLink<T> other) => other != null && sceneId.Equals(other.sceneId);
        
        public override bool Equals(object obj) => obj is ContextLink<T> other && sceneId.Equals(other.sceneId);
        
        public override int GetHashCode() => sceneId;
    }
}