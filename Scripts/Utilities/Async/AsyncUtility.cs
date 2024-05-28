﻿#if UNITASK_ENABLE
using Cysharp.Threading.Tasks;

#else
using System.Threading.Tasks;
using UnityEngine;
#endif

namespace TinyMVC.Utilities.Async {
    public static class AsyncUtility {
        #if UNITASK_ENABLE

        public static async UniTask<bool> Delay(int millisecondsDelay, AsyncCancellation cancellation, bool ignoreTimeScale = false) {
            await UniTask.Delay(millisecondsDelay, ignoreTimeScale);

            return cancellation.isCancel;
        }

        #else
        public static async Task<bool> Delay(int millisecondsDelay, AsyncCancellation cancellation, bool ignoreTimeScale = false) {
            if (ignoreTimeScale) {
                await Task.Delay(millisecondsDelay);
            } else {
                await DelayWithTimeScale(millisecondsDelay);
            }

            return cancellation.isCancel;
        }

        private static async Task DelayWithTimeScale(int millisecondsDelay) {
            while (millisecondsDelay > 0) {
                await Task.Delay(1);

                int step = (int)(Time.deltaTime * 1000);
                millisecondsDelay -= step;
            }
        }

        #endif
    }
}