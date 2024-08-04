namespace TinyMVC.Modules.Analytics {
    public sealed class AnalyticsParameter {
        internal readonly string parameterName;
        internal readonly string stringValue;
        internal readonly long longValue;
        internal readonly double doubleValue;
        internal readonly ValueType type;
        
        internal enum ValueType : byte {
            String,
            Long,
            Double
        }
        
        public AnalyticsParameter(string parameterName, string parameterValue) {
            this.parameterName = parameterName;
            stringValue = parameterValue;
            type = ValueType.String;
        }
        
        public AnalyticsParameter(string parameterName, long parameterValue) {
            this.parameterName = parameterName;
            stringValue = $"{parameterValue}";
            longValue = parameterValue;
            type = ValueType.Long;
        }
        
        public AnalyticsParameter(string parameterName, double parameterValue) {
            this.parameterName = parameterName;
            stringValue = $"{parameterValue}";
            doubleValue = parameterValue;
            type = ValueType.Double;
        }
        
        public override string ToString() => $"{parameterName}:{stringValue}";
    }
}