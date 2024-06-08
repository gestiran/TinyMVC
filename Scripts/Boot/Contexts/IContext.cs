using System.Threading.Tasks;

namespace TinyMVC.Boot.Contexts {
    public interface IContext {
        internal Task Create();
        
        internal Task InitAsync(ProjectContext context, int sceneId);
        
        internal void Unload();
    }
}