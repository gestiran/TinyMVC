using System.Threading.Tasks;
using TinyMVC.Boot.Contexts;

namespace TinyMVC.Boot.Empty {
    public sealed class ControllersEmptyContext : ControllersContext {
        internal ControllersEmptyContext() { }

        protected override async Task Create() { }
    }
}