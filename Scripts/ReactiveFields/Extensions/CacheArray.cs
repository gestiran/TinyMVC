// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;

namespace TinyMVC.ReactiveFields.Extensions {
    internal static class CacheArray<T> {
        private static readonly Data[] _8;
        private static readonly Data[] _32;
        private static readonly Data[] _128;
        private static readonly Data[] _512;
        
        private const int _LIMIT = 8;
        
        public sealed class Data : IEquatable<Data> {
            public bool isBusy;
            
            public readonly int id;
            public readonly T[] array;
            
            public Data(int id, int size) {
                this.id = id;
                array = new T[size];
            }
            
            public bool Equals(Data other) => other != null && other.id == id;
            
            public override bool Equals(object obj) => obj is Data other && other.id == id;
            
            public override int GetHashCode() => id;
        }
        
        static CacheArray() {
            _8 = CreateCache(8);
            _32 = CreateCache(32);
            _128 = CreateCache(128);
            _512 = CreateCache(512);
        }
        
        public static bool TryGet(int size, out Data cache) {
            if (size <= 8 && TryGet(_8, out cache)) {
                cache.isBusy = true;
                return true;
            }
            
            if (size <= 32 && TryGet(_32, out cache)) {
                cache.isBusy = true;
                return true;
            }
            
            if (size <= 128 && TryGet(_128, out cache)) {
                cache.isBusy = true;
                return true;
            }
            
            if (size <= 512 && TryGet(_512, out cache)) {
                cache.isBusy = true;
                return true;
            }
            
            cache = null;
            return false;
        }
        
        private static bool TryGet(Data[] pool, out Data cache) {
            for (int i = 0; i < pool.Length; i++) {
                if (pool[i].isBusy) {
                    continue;
                }
                
                cache = pool[i];
                return true;
            }
            
            cache = null;
            return false;
        }
        
        private static Data[] CreateCache(int size) {
            Data[] cache = new Data[_LIMIT];
            
            for (int i = 0; i < cache.Length; i++) {
                cache[i] = new Data(i, size);
            }
            
            return cache;
        }
    }
}