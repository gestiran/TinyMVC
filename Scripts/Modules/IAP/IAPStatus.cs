// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

namespace TinyMVC.Modules.IAP {
    public enum IAPStatus : byte {
        Success,
        FailedNetworkNotReachable,
        FailedInternal,
        FailedTimeout
    }
}