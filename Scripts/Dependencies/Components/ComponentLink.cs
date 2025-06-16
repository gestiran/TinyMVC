namespace TinyMVC.Dependencies.Components {
    public sealed class ComponentLink<T> {
        public readonly Model model;
        public readonly T component;
        
        internal ComponentLink(Model model, T component) {
            this.model = model;
            this.component = component;
        }
    }
}