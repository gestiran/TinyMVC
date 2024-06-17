#if GOOGLE_FIREBASE_ANALYTICS
using Firebase.Analytics;

namespace TinyMVC.Modules.Analytics {
    public static class AnalyticsParameterExtension {
        public static Parameter[] ToParameters(this AnalyticsParameter[] parameters) {
            Parameter[] result = new Parameter[parameters.Length];
            
            for (int i = 0; i < parameters.Length; i++) {
                result[i] = parameters[i];
            }
            
            return result;
        }
    }
}
#endif