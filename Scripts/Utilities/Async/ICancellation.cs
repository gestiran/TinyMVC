﻿namespace TinyMVC.Utilities.Async {
    public interface ICancellation {
        public bool isCancel { get; }
        
        public void Cancel();
    }
}