using TinyMVC.Dependencies;

namespace TinyMVC.Boot.Binding {
    public abstract class BinderMod {
        internal abstract void BindInternal();
    }
    
    public abstract class BinderMod<T> : BinderMod where T : IDependency {
        internal override void BindInternal() {
            if (ProjectContext.data.Get(out T model)) {
                Bind(model);
            }
        }
        
        protected abstract void Bind(T model);
    }
}