namespace TinyMVC.Modules.ADS {
    public sealed class AsyncCancellation {
        public bool isCancel { get; private set; }
        
        public void Cancel() => isCancel = true;
    }
}