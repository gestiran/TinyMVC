using System;
using TinyMVC.Boot.Contexts;

namespace TinyMVC.Boot.Empty {
    [Serializable]
    public sealed class ViewsEmptyContext : ViewsContext {
        protected override void Create() { }
    }
}