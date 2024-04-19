using JetBrains.Annotations;
using TinyMVC.Dependencies;
using TinyMVC.Loop;

namespace TinyMVC.Views {
    public interface IViewConnector {
        public T ConnectView<T>([NotNull] T view) where T : class, IView;
        public T ConnectView<T>([NotNull] T view, UnloadPool pool) where T : class, IView;
        public void ConnectView([NotNull] params IView[] views);
        public void ConnectView([NotNull] params View[] views);
        public void ConnectView<T>(T[] views, [NotNull] params IDependency[] dependencies) where T : class, IView, IResolving;
        public void ConnectView(UnloadPool pool, [NotNull] params IView[] views);
        public T ConnectView<T>([NotNull] T view, IDependency dependency) where T : class, IView, IResolving;
        public T ConnectView<T>([NotNull] T view, UnloadPool pool, IDependency dependency) where T : class, IView, IResolving;
        public T ConnectView<T>([NotNull] T view, [NotNull] params IDependency[] dependencies) where T : class, IView, IResolving;
        public T ConnectView<T>([NotNull] T view, UnloadPool pool, [NotNull] params IDependency[] dependencies) where T : class, IView, IResolving;
        public void DisconnectView<T>([NotNull] T view) where T : class, IView;
        public void DisconnectView([NotNull] params IView[] views);
    }
}