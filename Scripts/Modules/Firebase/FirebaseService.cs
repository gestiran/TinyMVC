using System.Threading.Tasks;
using UnityEngine;

#if GOOGLE_FIREBASE_APP
using Firebase;
#endif

namespace TinyMVC.Modules.Firebase {
    public static class FirebaseService {
    #if GOOGLE_FIREBASE_APP
        public static DependencyStatus status { get; private set; }
        
        static FirebaseService() => status = DependencyStatus.UnavailableDisabled;
        
        public static async Task Initialize() {
            status = await FirebaseApp.CheckAndFixDependenciesAsync();
            
            if (status == DependencyStatus.Available) {
                FirebaseApp _ = FirebaseApp.DefaultInstance;
                Debug.Log($"FirebaseService: {status}");
            } else {
                Debug.LogError($"FirebaseService: {status}");
            }
            
            await Task.Yield();
        }
    #endif
    }
}