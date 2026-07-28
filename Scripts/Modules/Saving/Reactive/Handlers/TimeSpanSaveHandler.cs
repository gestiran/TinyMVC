// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;

namespace TinyMVC.Modules.Saving.Reactive.Handlers {
    public sealed class TimeSpanSaveHandler : ISaveHandler<TimeSpan> {
        public static TimeSpanSaveHandler instance { get; private set; }
        
        static TimeSpanSaveHandler() => instance = new TimeSpanSaveHandler();
        
        private TimeSpanSaveHandler() { }
        
        public void Save(TimeSpan value, string key) {
            DefaultSaveHandler<long>.instance.Save(value.Ticks, key);
        }
        
        public void Save(TimeSpan value, string key, params string[] group) {
            DefaultSaveHandler<long>.instance.Save(value.Ticks, key, group);
        }
        
        public bool TryLoad(TimeSpan defaultValue, out TimeSpan value, string key) {
            if (DefaultSaveHandler<long>.instance.TryLoad(defaultValue.Ticks, out long savedTicks, key)) {
                value = new TimeSpan(savedTicks);
                return true;
            }
            
            value = defaultValue;
            return false;
        }
        
        public bool TryLoad(TimeSpan defaultValue, out TimeSpan value, string key, params string[] group) {
            if (DefaultSaveHandler<long>.instance.TryLoad(defaultValue.Ticks, out long savedTicks, key, group)) {
                value = new TimeSpan(savedTicks);
                return true;
            }
            
            value = defaultValue;
            return false;
        }
    }
}