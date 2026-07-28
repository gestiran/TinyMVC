// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;

namespace TinyMVC.Modules.Saving.Reactive.Handlers {
    public sealed class DateTimeSaveHandler : ISaveHandler<DateTime> {
        public static DateTimeSaveHandler instance { get; private set; }
        
        static DateTimeSaveHandler() => instance = new DateTimeSaveHandler();
        
        private DateTimeSaveHandler() { }
        
        public void Save(DateTime value, string key) {
            DefaultSaveHandler<long>.instance.Save(value.Ticks, key);
        }
        
        public void Save(DateTime value, string key, params string[] group) {
            DefaultSaveHandler<long>.instance.Save(value.Ticks, key, group);
        }
        
        public bool TryLoad(DateTime defaultValue, out DateTime value, string key) {
            if (DefaultSaveHandler<long>.instance.TryLoad(defaultValue.Ticks, out long savedTicks, key)) {
                value = new DateTime(savedTicks);
                return true;
            }
            
            value = defaultValue;
            return false;
        }
        
        public bool TryLoad(DateTime defaultValue, out DateTime value, string key, params string[] group) {
            if (DefaultSaveHandler<long>.instance.TryLoad(defaultValue.Ticks, out long savedTicks, key, group)) {
                value = new DateTime(savedTicks);
                return true;
            }
            
            value = defaultValue;
            return false;
        }
    }
}