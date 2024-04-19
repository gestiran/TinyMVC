using Random = UnityEngine.Random;

namespace TinyMVC.Dependencies.Extensions {
    public static class DependencyPoolExtension {
        public static T GetRandom<T>(this DependencyPool<T> pool) where T : IDependency => pool[Random.Range(0, pool.length - 1)];
    }
}