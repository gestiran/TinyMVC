using System;
using JetBrains.Annotations;
using UnityEngine;

#if GOOGLE_FIREBASE_ANALYTICS
using Firebase.Analytics;
#endif

namespace TinyMVC.Modules.Analytics {
    public sealed class AnalyticsModule : IApplicationModule {
        private readonly AnalyticsLog _log;
        
        public AnalyticsModule() => _log = new AnalyticsLog();
        
        [Obsolete("Can't send empty event!", true)]
        public void LogEvent() => Debug.LogError("Don't have parameters!");
        
        public void LogEvent(string eventName) => SendEvent(new AnalyticsEvent(eventName));
        
        public void LogEvent(string eventName, params AnalyticsParameter[] parameters) => SendEvent(new AnalyticsEvent(eventName, parameters));
        
        public void LogEvent(string eventName, string parameterName, string parameterValue) {
            SendEvent(new AnalyticsEvent(eventName, new AnalyticsParameter(parameterName, parameterValue)));
        }
        
        public void LogEvent(string eventName, string parameterName, long parameterValue) {
            SendEvent(new AnalyticsEvent(eventName, new AnalyticsParameter(parameterName, parameterValue)));
        }
        
        public void LogEvent(string eventName, string parameterName, double parameterValue) {
            SendEvent(new AnalyticsEvent(eventName, new AnalyticsParameter(parameterName, parameterValue)));
        }
        
        public void LogEvent(AnalyticsEvent data) => SendEvent(data);
        
        public void LogEvent([NotNull] params AnalyticsEvent[] data) {
            for (int i = 0; i < data.Length; i++) {
                SendEvent(data[i]);
            }
        }
        
        private void SendEvent(AnalyticsEvent data) {
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
    }
}