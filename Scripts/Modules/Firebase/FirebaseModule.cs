#if GOOGLE_FIREBASE_APP
using System.Threading.Tasks;
using Firebase;
using UnityEngine;

namespace TinyMVC.Modules.Firebase {
    public sealed class FirebaseModule : IApplicationModule {
        public DependencyStatus status { get; private set; }
        
        public FirebaseModule() => status = DependencyStatus.UnavailableDisabled;
        
        public async Task Initialize() {
            status = await FirebaseApp.CheckAndFixDependenciesAsync();
            
            if (status == DependencyStatus.Available) {
                FirebaseApp _ = FirebaseApp.DefaultInstance;
                Debug.Log($"FirebaseModule: {status}");
            } else {
                Debug.LogError($"FirebaseModule: {status}");
            }
            
            await Task.Yield();
        }
    }
}
#endif