using System.Collections.Generic;
using TinyMVC.Boot.Contexts;
using TinyMVC.Dependencies;

namespace TinyMVC.Boot.Empty {
    public sealed class ModelsEmptyContext : ModelsContext {
        internal ModelsEmptyContext() { }
        
        protected override void Bind() { }
        
        protected override void Create(List<IDependency> models) { }
    }
}