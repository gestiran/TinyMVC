// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using TinyMVC.Modules.Saving;

namespace TinyMVC.Modules.RateUs {
    public static class RateUsSaveUtility {
        private const string _GROUP = "API";
        private const string _IS_NEED_SHOW = "IsNeedRateUs";
        private const string _IS_FIRST_SHOW = "IsFirstRateUsShow";
        private const string _TO_RATE_US_TIME = "ToRateUsTime";
        
        public static bool LoadIsNeedShow(bool defaultValue) => SaveService.Load(defaultValue, _IS_NEED_SHOW, _GROUP);
        
        public static bool LoadIsFirstShow() => SaveService.Load(true, _IS_FIRST_SHOW, _GROUP);
        
        public static int LoadToRateUsTime(int defaultValue) => SaveService.Load(defaultValue, _TO_RATE_US_TIME, _GROUP);
        
        public static void SaveIsNeedShow(bool value) => SaveService.Save(value, _IS_NEED_SHOW, _GROUP);
        
        public static void SaveIsFirstShow(bool value) => SaveService.Save(value, _IS_FIRST_SHOW, _GROUP);
        
        public static void SaveToRateUsTime(int value) => SaveService.Save(value, _TO_RATE_US_TIME, _GROUP);
    }
}