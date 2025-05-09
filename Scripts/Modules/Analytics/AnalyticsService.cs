using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

#if GOOGLE_FIREBASE_ANALYTICS
using Firebase.Analytics;
using TinyMVC.Modules.Firebase;
#endif

namespace TinyMVC.Modules.Analytics {
    public static class AnalyticsService {
        private static readonly AnalyticsLog _log;
        
        static AnalyticsService() => _log = new AnalyticsLog();
        
        public static void ApplyConsent() {
        #if GOOGLE_FIREBASE_ANALYTICS
            if (FirebaseService.status != FirebaseStatus.Available) {
                Debug.Log("AnalyticsService: Apply consent failed, firebase not available!");
                return;
            }
            
            FirebaseAnalytics.SetConsent(GeneratePermissions(ConsentStatus.Granted));
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
            Debug.Log("AnalyticsService: Apply consent success!");
        #else
            Debug.Log("AnalyticsService: Apply consent failed, module is inactive!");
        #endif
        }
        
        public static void RejectConsent() {
        #if GOOGLE_FIREBASE_ANALYTICS
            if (FirebaseService.status != FirebaseStatus.Available) {
                Debug.Log("AnalyticsService: Reject consent failed, firebase not available!");
                return;
            }
            
            FirebaseAnalytics.SetConsent(GeneratePermissions(ConsentStatus.Denied));
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
            Debug.Log("AnalyticsService: Reject consent success!");
        #else
            Debug.Log("AnalyticsService: Reject consent failed, module is inactive!");
        #endif
        }
        
        [Obsolete("Can't send empty event!", true)]
        public static void LogEvent() => Debug.LogError("Don't have parameters!");
        
        public static void LogEvent(string eventName) => SendEvent(new AnalyticsEvent(eventName));
        
        public static void LogEvent(string eventName, params AnalyticsParameter[] parameters) => SendEvent(new AnalyticsEvent(eventName, parameters));
        
        public static void LogEvent(string eventName, string parameterName, string parameterValue) {
            SendEvent(new AnalyticsEvent(eventName, new AnalyticsParameter(parameterName, parameterValue)));
        }
        
        public static void LogEvent(string eventName, string parameterName, long parameterValue) {
            SendEvent(new AnalyticsEvent(eventName, new AnalyticsParameter(parameterName, parameterValue)));
        }
        
        public static void LogEvent(string eventName, string parameterName, double parameterValue) {
            SendEvent(new AnalyticsEvent(eventName, new AnalyticsParameter(parameterName, parameterValue)));
        }
        
        public static void LogEvent(AnalyticsEvent data) => SendEvent(data);
        
        public static void LogEvent([NotNull] params AnalyticsEvent[] data) {
            for (int i = 0; i < data.Length; i++) {
                SendEvent(data[i]);
            }
        }
        
        private static void SendEvent(AnalyticsEvent data) {
        #if GOOGLE_FIREBASE_ANALYTICS
            switch (data.eventType) {
                case AnalyticsEvent.EventType.EventOnly: FirebaseAnalytics.LogEvent(data.eventName); break;
                
                case AnalyticsEvent.EventType.WithParameter:
                    AnalyticsParameter parameter = data.parameters[0];
                    
                    switch (parameter.type) {
                        case AnalyticsParameter.ValueType.String: FirebaseAnalytics.LogEvent(data.eventName, parameter.parameterName, parameter.stringValue); break;
                        case AnalyticsParameter.ValueType.Long: FirebaseAnalytics.LogEvent(data.eventName, parameter.parameterName, parameter.longValue); break;
                        case AnalyticsParameter.ValueType.Double: FirebaseAnalytics.LogEvent(data.eventName, parameter.parameterName, parameter.doubleValue); break;
                    }
                    
                    break;
                
                case AnalyticsEvent.EventType.WithParameters: FirebaseAnalytics.LogEvent(data.eventName, data.parameters.ToParameters()); break;
            }
        #endif
            
            _log.LogEvent(data);
        }
        
    #if GOOGLE_FIREBASE_ANALYTICS
        private static Dictionary<ConsentType, ConsentStatus> GeneratePermissions(ConsentStatus state) {
            Dictionary<ConsentType, ConsentStatus> consent = new Dictionary<ConsentType, ConsentStatus>();
            
            consent.Add(ConsentType.AnalyticsStorage, state);
            consent.Add(ConsentType.AdStorage, state);
            consent.Add(ConsentType.AdUserData, state);
            consent.Add(ConsentType.AdPersonalization, state);
            return consent;
        }
    #endif
    }
}