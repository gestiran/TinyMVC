using System.Collections.Generic;
using TinyMVC.Boot.Contexts;
using TinyMVC.Dependencies;

namespace TinyMVC.Boot.Empty {
    public sealed class ParametersEmptyContext : ParametersContext {
        internal ParametersEmptyContext() { }

        protected override void Create(List<IDependency> parameters) { }
    }
}