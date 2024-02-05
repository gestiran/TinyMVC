using UnityEngine.SceneManagement;

namespace TinyMVC.Boot {
    internal interface IContext {
        internal void Create();

        internal void Init(ProjectContext context, ref Scene scene);

        internal void Unload();
    }
}