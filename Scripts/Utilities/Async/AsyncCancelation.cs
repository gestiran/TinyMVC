namespace TinyMVC.Utilities.Async {
    public sealed class AsyncCancellation : ICancellation {
        public bool isCancel => _isCancel;
        
        private bool _isCancel;
        
        public void Cancel() => _isCancel = true;
    }
}