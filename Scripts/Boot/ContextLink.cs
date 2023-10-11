using System;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

namespace TinyMVC.Boot {
    public abstract class ContextLink<T> : IEquatable<ContextLink<T>>, IEquatable<Scene> {
        [HideInEditorMode, HideInPlayMode]
        public readonly Scene scene;
        
        [ShowInInspector, LabelText("@scene.name"), HideInEditorMode, HideReferenceObjectPicker]
        public readonly T context;

        protected ContextLink(Scene scene, T context) {
            this.scene = scene;
            this.context = context;
        }

        public bool Equals(ContextLink<T> other) => other != null && scene.Equals(other.scene);

        public bool Equals(Scene other) => scene.Equals(other);

        public override bool Equals(object obj) => obj is ContextLink<T> other && scene.Equals(other.scene);

        public override int GetHashCode() => scene.GetHashCode();
    }
}