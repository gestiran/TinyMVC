namespace TinyMVC.Modules.IAP {
    public enum IAPStatus : byte {
        Success,
        FailedNetworkNotReachable,
        FailedInternal,
        FailedTimeout
    }
}