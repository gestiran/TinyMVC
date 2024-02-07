using System;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

namespace TinyMVC.Boot.Helpers {
    internal abstract class ContextLink<T> : IEquatable<ContextLink<T>> {
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [HideInEditorMode, HideInPlayMode]
    #endif
        public readonly int sceneId;

    #if ODIN_INSPECTOR && UNITY_EDITOR
        private string _label;
            
        [ShowInInspector, Title("@" + nameof(_label)), InlineProperty, HideLabel, HideInEditorMode, HideReferenceObjectPicker, HideDuplicateReferenceBox]
    #endif
        public readonly T context;

        protected ContextLink(int sceneId, T context) {
            this.sceneId = sceneId;
            this.context = context;
        #if ODIN_INSPECTOR && UNITY_EDITOR
            _label = SceneManager.GetSceneByBuildIndex(sceneId).name;
        #endif
        }

        public bool Equals(ContextLink<T> other) => other != null && sceneId.Equals(other.sceneId);
            
        public override bool Equals(object obj) => obj is ContextLink<T> other && sceneId.Equals(other.sceneId);

        public override int GetHashCode() => sceneId;
    }
}