using UnityEngine;

namespace TinyMVC.Views {
    public abstract class DontDestroyView : MonoBehaviour, IView {
        public virtual void Init() => DontDestroyOnLoad(this);
    }
}