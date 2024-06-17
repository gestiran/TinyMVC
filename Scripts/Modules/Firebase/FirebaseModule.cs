#if GOOGLE_FIREBASE_APP
using System.Threading.Tasks;
using Firebase;
using Firebase.Extensions;
using UnityEngine;

namespace TinyMVC.Modules.Firebase {
    public sealed class FirebaseModule : IApplicationModule {
        public DependencyStatus status { get; private set; }
        
        private const int _MAX_DELAY = 3000;
        private const int _STEP = 100;
        
        public FirebaseModule() {
            status = DependencyStatus.UnavailableDisabled;
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(OnFixedDependencies);
        }
        
        public async Task WaitInitialization() {
            int time = _MAX_DELAY;
            
            while (time > 0) {
                if (status == DependencyStatus.Available) {
                    return;
                }
                
                await Task.Delay(_STEP);
                time -= _STEP;
            }
        }
        
        private void OnFixedDependencies(Task<DependencyStatus> result) {
            status = result.Result;
            
            if (status == DependencyStatus.Available) {
                FirebaseApp _ = FirebaseApp.DefaultInstance;
                Debug.Log($"FirebaseModule: {status}");
            } else {
                Debug.LogError($"FirebaseModule: {status}");
            }
        }
    }
}
#endif