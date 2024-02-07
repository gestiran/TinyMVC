namespace TinyMVC.Boot.Contexts {
    internal interface IContext {
        internal void Create();

        internal void Init(ProjectContext context, int sceneId);

        internal void Unload();
    }
}