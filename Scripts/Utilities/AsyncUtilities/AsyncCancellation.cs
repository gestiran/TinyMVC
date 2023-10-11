namespace TinyMVC.Utilities.AsyncUtilities {
    public class AsyncCancellation {
        public bool isCancel => _isCancel;

        private bool _isCancel;

        public void Cancel() => _isCancel = true;
    }
}