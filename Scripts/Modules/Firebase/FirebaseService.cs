// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Threading.Tasks;
using UnityEngine;

#if GOOGLE_FIREBASE_APP
using System;
using Firebase;
#endif

namespace TinyMVC.Modules.Firebase {
    public static class FirebaseService {
    #if GOOGLE_FIREBASE_APP
        public static FirebaseStatus status { get; private set; }
        
        static FirebaseService() => status = FirebaseStatus.UnavailableDisabled;
        
        public static Task Initialize() => Initialize(_ => { });
        
        public static async Task Initialize(Action<FirebaseStatus> onComplete) {
            DependencyStatus dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
            status = (FirebaseStatus)(int)dependencyStatus;
            
            if (status == FirebaseStatus.Available) {
                FirebaseApp _ = FirebaseApp.DefaultInstance;
                Debug.Log($"FirebaseService: {status}");
            } else {
                Debug.LogError($"FirebaseService: {status}");
            }
            
            await Task.Yield();
            
            onComplete.Invoke(status);
        }
    #endif
    }
}