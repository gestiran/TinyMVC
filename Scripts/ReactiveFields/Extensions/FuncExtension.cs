using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace TinyMVC.ReactiveFields.Extensions {
    public static class FuncExtension {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Invoke(this List<Func<bool>> actions, bool target = true) {
            if (actions.Count <= 0) {
                return;
            }
            
            Func<bool>[] temp = new Func<bool>[actions.Count];
            int i;
            
            for (i = 0; i < actions.Count; i++) {
                temp[i] = actions[i];
            }
            
            for (i = 0; i < temp.Length; i++) {
                if (temp[i].Invoke() == target) {
                    break;
                }
            }
        }
    }
}