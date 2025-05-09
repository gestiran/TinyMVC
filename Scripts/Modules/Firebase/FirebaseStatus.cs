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