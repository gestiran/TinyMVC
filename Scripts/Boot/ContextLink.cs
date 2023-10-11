using System;
using UnityEngine.SceneManagement;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Boot {
    public abstract class ContextLink<T> : IEquatable<ContextLink<T>>, IEquatable<Scene> {
    #if ODIN_INSPECTOR
        [HideInEditorMode, HideInPlayMode]
    #endif
        public readonly Scene scene;

    #if ODIN_INSPECTOR
        [ShowInInspector, LabelText("@" + nameof(scene) + ".name"), HideInEditorMode, HideReferenceObjectPicker]
    #endif
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