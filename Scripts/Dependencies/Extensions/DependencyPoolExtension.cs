// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using UnityRandom = UnityEngine.Random;

namespace TinyMVC.Dependencies.Extensions {
    public static class DependencyPoolExtension {
        public static T GetRandom<T>(this DependencyPool<T> pool) where T : IDependency => pool[UnityRandom.Range(0, pool.length - 1)];
    }
}