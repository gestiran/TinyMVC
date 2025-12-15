// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using TinyMVC.Modules.Saving.Reactive.Handlers;
using TinyReactive.Fields;

namespace TinyMVC.Modules.Saving.Reactive {
    public class ObservedSave<T> : Observed<T> {
        private ActionListener<T> _save;
        
        public ObservedSave(string key) : this(default, DefaultSaveHandler<T>.instance, key) { }
        
        public ObservedSave(T defaultValue, string key) : this(defaultValue, DefaultSaveHandler<T>.instance, key) { }
        
        public ObservedSave(T defaultValue, ISaveHandler<T> handler, string key) : base(defaultValue) {
            _value = handler.Load(defaultValue, key);
            _save = newValue => handler.Save(newValue, key);
        }
        
        public ObservedSave(string key, params string[] group) : this(default, DefaultSaveHandler<T>.instance, key, group) { }
        
        public ObservedSave(T defaultValue, string key, params string[] group) : this(defaultValue, DefaultSaveHandler<T>.instance, key, group) { }
        
        public ObservedSave(T defaultValue, ISaveHandler<T> handler, string key, params string[] group) : base(defaultValue) {
            _value = handler.Load(defaultValue, key, group);
            _save = newValue => handler.Save(newValue, key, group);
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