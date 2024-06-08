using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace TinyMVC.Utilities.Async {
    public static class CancellationExtension {
        public static void Cancel<T>(this List<T> cancellations) where T : ICancellation {
            for (int cancellationId = 0; cancellationId < cancellations.Count; cancellationId++) {
                cancellations[cancellationId].Cancel();
            }
        }
        
        public static void Cancel<T>(this T[] cancellations) where T : ICancellation {
            for (int cancellationId = 0; cancellationId < cancellations.Length; cancellationId++) {
                cancellations[cancellationId].Cancel();
            }
        }
        
        public static T UpdateAsync<T>([CanBeNull] this T cancellation, Action<T> action) where T : ICancellation, new() {
            if (cancellation != null) {
                cancellation.Cancel();
            }
            
            T result = new T();
            action(result);
            
            return result;
        }
    }
}