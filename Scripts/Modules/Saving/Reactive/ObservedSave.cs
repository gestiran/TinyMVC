// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using TinyReactive.Fields;

namespace TinyMVC.Modules.Saving.Reactive {
    public class ObservedSave<T> : Observed<T> {
        private ActionListener<T> _save;
        
        public ObservedSave(string key) : this(default(T), key) { }
        
        public ObservedSave(T defaultValue, string key) : base(defaultValue) {
            _value = SaveService.Load(defaultValue, key);
            _save = newValue => SaveService.Save(newValue, key);
        }
        
        public ObservedSave(string key, params string[] group) : this(default, key, group) { }
        
        public ObservedSave(T defaultValue, string key, params string[] group) : base(defaultValue) {
            _value = SaveService.Load(defaultValue, key, group);
            _save = newValue => SaveService.Save(newValue, key, group);
        }
        
        public override void Set(T newValue) {
            base.Set(newValue);
            _save.Invoke(newValue);
        }
        
        public override void Unload() {
            base.Unload();
            _save = EmptySave;
        }
        
        private static void EmptySave(T newValue) { }
    }
}