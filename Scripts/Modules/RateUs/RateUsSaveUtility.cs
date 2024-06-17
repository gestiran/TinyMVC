using TinyMVC.Modules.Saving;

namespace TinyMVC.Modules.RateUs {
    public static class RateUsSaveUtility {
        private const string _IS_NEED_SHOW = "IsNeedRateUs";
        private const string _IS_FIRST_SHOW = "IsFirstRateUsShow";
        private const string _TO_RATE_US_TIME = "ToRateUsTime";
        
        public static bool LoadIsNeedShow(bool defaultValue) => API<SaveModule>.module.Load(defaultValue, _IS_NEED_SHOW, API.SAVE_GROUP);
        
        public static bool LoadIsFirstShow() => API<SaveModule>.module.Load(true, _IS_FIRST_SHOW, API.SAVE_GROUP);
        
        public static int LoadToRateUsTime(int defaultValue) => API<SaveModule>.module.Load(defaultValue, _TO_RATE_US_TIME, API.SAVE_GROUP);
        
        public static void SaveIsNeedShow(bool value) => API<SaveModule>.module.Save(value, _IS_NEED_SHOW, API.SAVE_GROUP);
        
        public static void SaveIsFirstShow(bool value) => API<SaveModule>.module.Save(value, _IS_FIRST_SHOW, API.SAVE_GROUP);
        
        public static void SaveToRateUsTime(int value) => API<SaveModule>.module.Save(value, _TO_RATE_US_TIME, API.SAVE_GROUP);
    }
}