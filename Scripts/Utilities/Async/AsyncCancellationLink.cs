namespace TinyMVC.Utilities.Async {
    public sealed class AsyncCancellationLink : ICancellation {
        public bool isCancel => _isCancel || _root.isCancel;

        private bool _isCancel;
        
        private readonly AsyncCancellation _root;
        
        public AsyncCancellationLink(AsyncCancellation root) => _root = root;
        
        public void Cancel() => _isCancel = true;
    }
}