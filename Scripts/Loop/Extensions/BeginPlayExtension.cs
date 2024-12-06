using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace TinyMVC.Loop.Extensions {
    public static class BeginPlayExtension {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TryBeginPlay<T>(this ICollection<T> collection) {
            foreach (T obj in collection) {
                obj.TryBeginPlaySingle();
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task TryBeginPlayAsync<T>(this ICollection<T> collection) {
            foreach (T obj in collection) {
                await obj.TryBeginPlayAsyncSingle();
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BeginPlay<T>(this ICollection<T> collection) where T : IBeginPlay {
            foreach (T obj in collection) {
                obj.BeginPlay();
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task BeginPlayAsync<T>(this ICollection<T> collection) where T : IBeginPlayAsync {
            foreach (T obj in collection) {
                await obj.BeginPlay();
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TryBeginPlaySingle<T>(this T obj) {
            if (obj is IBeginPlayAsync otherAsync) {
                otherAsync.BeginPlay();
            } else if (obj is IBeginPlay other) {
                other.BeginPlay();
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task TryBeginPlayAsyncSingle<T>(this T obj)  {
            if (obj is IBeginPlayAsync otherAsync) {
                await otherAsync.BeginPlay();
            } else if (obj is IBeginPlay other) {
                other.BeginPlay();
            }
        }
    }
}