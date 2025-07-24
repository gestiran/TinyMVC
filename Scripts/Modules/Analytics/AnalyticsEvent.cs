// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

namespace TinyMVC.Modules.Analytics {
    public readonly struct AnalyticsEvent {
        public readonly string eventName;
        public readonly AnalyticsParameter[] parameters;
        public readonly EventType eventType;
        
        public enum EventType : byte {
            EventOnly,
            WithParameter,
            WithParameters
        }
        
        public AnalyticsEvent(string eventName) {
            this.eventName = eventName;
            parameters = null;
            eventType = EventType.EventOnly;
        }
        
        public AnalyticsEvent(string eventName, AnalyticsParameter parameter) {
            this.eventName = eventName;
            parameters = new[] { parameter };
            eventType = EventType.WithParameter;
        }
        
        public AnalyticsEvent(string eventName, params AnalyticsParameter[] parameters) {
            this.eventName = eventName;
            this.parameters = parameters;
            eventType = EventType.WithParameters;
        }
    }
}