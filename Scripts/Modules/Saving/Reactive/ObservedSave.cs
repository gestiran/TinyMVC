// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
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
    
    public class ObservedSave<T1, T2> : Observed<T1> {
        private ActionListener<T1> _save;
        
        public ObservedSave(Func<T1, T2> to, Func<T2, T1> from, string key) : this(to, from, default(T1), key) { }
        
        public ObservedSave(Func<T1, T2> to, Func<T2, T1> from, T1 defaultValue, string key) : base(defaultValue) {
            if (SaveService.Has(key)) {
                _value = from.Invoke(SaveService.Load(default(T2), key));
            } else {
                _value = defaultValue;
            }
            
            _save = newValue => SaveService.Save(to(newValue), key);
        }
        
        public ObservedSave(Func<T1, T2> to, Func<T2, T1> from, string key, params string[] group) : this(to, from, default, key, group) { }
        
        public ObservedSave(Func<T1, T2> to, Func<T2, T1> from, T1 defaultValue, string key, params string[] group) : base(defaultValue) {
            if (SaveService.Has(key, group)) {
                _value = from.Invoke(SaveService.Load(default(T2), key, group));
            } else {
                _value = defaultValue;
            }
            
            _save = newValue => SaveService.Save(to.Invoke(newValue), key, group);
        }
        
        public override void Set(T1 newValue) {
            base.Set(newValue);
            _save.Invoke(newValue);
        }
        
        public override void Unload() {
            base.Unload();
            _save = EmptySave;
        }
        
        private static void EmptySave(T1 newValue) { }
    }
}