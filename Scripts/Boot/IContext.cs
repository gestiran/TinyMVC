using UnityEngine.SceneManagement;

namespace TinyMVC.Boot {
    public interface IContext {
        public void Create();

        public void Init(ProjectBootstrap projectContext, Scene scene);

        public void Unload();
    }
}