namespace TinyMVC.Dependencies {
    /// <summary> Marker of an object that needs dependencies </summary>
    /// <remarks> Requires fields with the <see cref="Inject"/> attribute </remarks>
    public interface IApplyResolving : IResolving {
        /// <summary> Called after all dependencies have been added </summary>
        public void ApplyResolving();
    }
}