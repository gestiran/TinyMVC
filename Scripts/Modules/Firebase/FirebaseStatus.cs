// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

namespace TinyMVC.Modules.Firebase {
    public enum FirebaseStatus : byte {
        Available,
        UnavailableDisabled,
        UnavailableInvalid,
        UnavilableMissing,
        UnavailablePermission,
        UnavailableUpdaterequired,
        UnavailableUpdating,
        UnavailableOther,
    }
}