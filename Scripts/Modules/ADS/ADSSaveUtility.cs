using TinyMVC.Modules.Saving;

namespace TinyMVC.Modules.ADS {
    public static class ADSSaveUtility {
        private const string _IS_NO_ADS = "IsNoAdsPurchased";
        private const string _REMAINING_BANNER_TIME = "RemainBannerTime";
        private const string _USER_AGE = "UserAge";
        private const string _BANNER_REWARDS_COUNT = "BannerRewardsCount";
        private const string _INTERSTITIAL_COOLDOWN = "InterstitialCooldown";
        private const string _BANNER_VISIBILITY = "BannerVisibility";
        private const string _TOKENS_COUNT = "TokensCount";
        
        public static bool HasSavedAge() => SaveService.Has(_USER_AGE, API.SAVE_GROUP);
        
        public static void SaveIsNoADS(bool value) => SaveService.Save(value, _IS_NO_ADS, API.SAVE_GROUP);
        
        public static void SaveAge(int value) => SaveService.Save(value, _USER_AGE, API.SAVE_GROUP);
        
        public static void SaveRemainingBannerTime(int value) => SaveService.Save(value, _REMAINING_BANNER_TIME, API.SAVE_GROUP);
        
        public static void SaveBannerRewardsCount(int value) => SaveService.Save(value, _BANNER_REWARDS_COUNT, API.SAVE_GROUP);
        
        public static void SaveWithoutInterstitialTime(int value) => SaveService.Save(value, _INTERSTITIAL_COOLDOWN, API.SAVE_GROUP);
        
        public static void SaveBannerVisibility(bool value) => SaveService.Save(value, _BANNER_VISIBILITY, API.SAVE_GROUP);
        
        public static void SaveTokensCount(int value) => SaveService.Save(value, _TOKENS_COUNT, API.SAVE_GROUP);
        
        public static bool LoadIsNoADS() => SaveService.Load(false, _IS_NO_ADS, API.SAVE_GROUP);
        
        public static int LoadAge() => SaveService.Load(16, _USER_AGE, API.SAVE_GROUP);
        
        public static int LoadRemainingBannerTime(int defaultValue) => SaveService.Load(defaultValue, _REMAINING_BANNER_TIME, API.SAVE_GROUP);
        
        public static int LoadBannerRewardsCount() => SaveService.Load(0, _BANNER_REWARDS_COUNT, API.SAVE_GROUP);
        
        public static int LoadWithoutInterstitialTime(int defaultValue) => SaveService.Load(defaultValue, _INTERSTITIAL_COOLDOWN, API.SAVE_GROUP);
        
        public static bool LoadBannerVisibility() => SaveService.Load(false, _BANNER_VISIBILITY, API.SAVE_GROUP);
        
        public static int LoadTokensCount(int value) => SaveService.Load(value, _TOKENS_COUNT, API.SAVE_GROUP);
    }
}