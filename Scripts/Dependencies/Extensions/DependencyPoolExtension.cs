using UnityRandom = UnityEngine.Random;

namespace TinyMVC.Dependencies.Extensions {
    public static class DependencyPoolExtension {
        public static T GetRandom<T>(this DependencyPool<T> pool) where T : IDependency => pool[UnityRandom.Range(0, pool.length - 1)];
    }
}