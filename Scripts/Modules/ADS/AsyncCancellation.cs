namespace TinyMVC.Modules.ADS {
    internal sealed class AsyncCancellation {
        public bool isCancel { get; private set; }
        
        public void Cancel() => isCancel = true;
    }
}