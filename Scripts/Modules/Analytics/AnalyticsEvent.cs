namespace TinyMVC.Modules.Analytics {
    public readonly struct AnalyticsEvent {
        public readonly string eventName;
        public readonly AnalyticsParameter[] parameters;
        public readonly EventType eventType;

        public enum EventType : byte {
            EventOnly,
            WithParameters
        }

        public AnalyticsEvent(string eventName) {
            this.eventName = eventName;
            parameters = null;
            eventType = EventType.EventOnly;
        }
        
        public AnalyticsEvent(string eventName, params AnalyticsParameter[] parameters) {
            this.eventName = eventName;
            this.parameters = parameters;
            eventType = EventType.WithParameters;
        }
    }
}