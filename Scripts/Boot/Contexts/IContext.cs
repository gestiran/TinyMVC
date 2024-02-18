using System.Threading.Tasks;

namespace TinyMVC.Boot.Contexts {
    public interface IContext {
        internal void Create();

        internal Task InitAsync(ProjectContext context, int sceneId);

        internal void Unload();
    }
}