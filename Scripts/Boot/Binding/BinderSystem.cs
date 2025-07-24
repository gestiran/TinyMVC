// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using TinyMVC.Boot.Contexts;
using TinyMVC.Dependencies;

namespace TinyMVC.Boot.Binding {
    public abstract class BinderSystem {
        private ModelsContext _context;
        
        internal void Connect(ModelsContext context) {
            _context = context;
            Bind();
        }
        
        protected abstract void Bind();
        
        protected void Add<T>(T binder) where T : Binder {
            if (binder is IBindConditions conditions && conditions.IsNeedBinding() == false) {
                return;
            }
            
            IDependency dependency = binder.GetDependency();
            ProjectContext.data.Add(_context.key, dependency);
            _context.dependenciesBinded.Add(dependency);
        }
        
        protected void Add(BinderMod binder) {
            if (binder is IBindConditions conditions && conditions.IsNeedBinding() == false) {
                return;
            }
            
            binder.BindInternal();
        }
    }
}