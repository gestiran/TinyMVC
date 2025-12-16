// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;

namespace TinyMVC.Modules.Saving.Reactive.Handlers {
    public sealed class DateTimeSaveHandler : ISaveHandler<DateTime> {
        public static DateTimeSaveHandler instance { get; private set; }
        
        static DateTimeSaveHandler() => instance = new DateTimeSaveHandler();
        
        private DateTimeSaveHandler() { }
        
        public void Save(DateTime value, string key) => SaveService.Save(value.Ticks, key);
        
        public void Save(DateTime value, string key, params string[] group) => SaveService.Save(value.Ticks, key, group);
        
        public DateTime Load(DateTime defaultValue, string key) => new DateTime(SaveService.Load(defaultValue.Ticks, key));
        
        public DateTime Load(DateTime defaultValue, string key, params string[] group) => new DateTime(SaveService.Load(defaultValue.Ticks, key, group));
    }
}