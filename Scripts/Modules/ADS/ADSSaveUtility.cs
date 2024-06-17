using TinyMVC.Modules.Saving;

namespace TinyMVC.Modules.ADS {
    public static class ADSSaveUtility {
        private const string _IS_NO_ADS = "IsNoAdsPurchased";
        private const string _REMAINING_BANNER_TIME = "RemainBannerTime";
        private const string _USER_AGE = "UserAge";
        private const string _BANNER_REWARDS_COUNT = "BannerRewardsCount";
        private const string _INTERSTITIAL_COOLDOWN = "InterstitialCooldown";
        private const string _BANNER_VISIBILITY = "BannerVisibility";
        
        public static bool HasSavedAge() => API<SaveModule>.module.Has(_USER_AGE, API.SAVE_GROUP);
        
        public static void SaveIsNoADS(bool value) => API<SaveModule>.module.Save(value, _IS_NO_ADS, API.SAVE_GROUP);
        
        public static void SaveAge(int value) => API<SaveModule>.module.Save(value, _USER_AGE, API.SAVE_GROUP);
        
        public static void SaveRemainingBannerTime(int value) => API<SaveModule>.module.Save(value, _REMAINING_BANNER_TIME, API.SAVE_GROUP);
        
        public static void SaveBannerRewardsCount(int value) => API<SaveModule>.module.Save(value, _BANNER_REWARDS_COUNT, API.SAVE_GROUP);
        
        public static void SaveWithoutInterstitialTime(int value) => API<SaveModule>.module.Save(value, _INTERSTITIAL_COOLDOWN, API.SAVE_GROUP);
        
        public static void SaveBannerVisibility(bool value) => API<SaveModule>.module.Save(value, _BANNER_VISIBILITY, API.SAVE_GROUP);
        
        public static bool LoadIsNoADS() => API<SaveModule>.module.Load(false, _IS_NO_ADS, API.SAVE_GROUP);
        
        public static int LoadAge() => API<SaveModule>.module.Load(16, _USER_AGE, API.SAVE_GROUP);
        
        public static int LoadRemainingBannerTime(int defaultValue) => API<SaveModule>.module.Load(defaultValue, _REMAINING_BANNER_TIME, API.SAVE_GROUP);
        
        public static int LoadBannerRewardsCount() => API<SaveModule>.module.Load(0, _BANNER_REWARDS_COUNT, API.SAVE_GROUP);
        
        public static int LoadWithoutInterstitialTime(int defaultValue) => API<SaveModule>.module.Load(defaultValue, _INTERSTITIAL_COOLDOWN, API.SAVE_GROUP);
        
        public static bool LoadBannerVisibility() => API<SaveModule>.module.Load(false, _BANNER_VISIBILITY, API.SAVE_GROUP);
    }
}