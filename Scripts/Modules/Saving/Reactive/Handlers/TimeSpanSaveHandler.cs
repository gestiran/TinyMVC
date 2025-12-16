// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;

namespace TinyMVC.Modules.Saving.Reactive.Handlers {
    public sealed class TimeSpanSaveHandler : ISaveHandler<TimeSpan> {
        public static TimeSpanSaveHandler instance { get; private set; }
        
        static TimeSpanSaveHandler() => instance = new TimeSpanSaveHandler();
        
        private TimeSpanSaveHandler() { }
        
        public void Save(TimeSpan value, string key) => SaveService.Save(value.Ticks, key);
        
        public void Save(TimeSpan value, string key, params string[] group) => SaveService.Save(value.Ticks, key, group);
        
        public TimeSpan Load(TimeSpan defaultValue, string key) => new TimeSpan(SaveService.Load(defaultValue.Ticks, key));
        
        public TimeSpan Load(TimeSpan defaultValue, string key, params string[] group) => new TimeSpan(SaveService.Load(defaultValue.Ticks, key, group));
    }
}