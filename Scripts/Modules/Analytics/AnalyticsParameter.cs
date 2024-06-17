#if GOOGLE_FIREBASE_ANALYTICS
using Firebase.Analytics;
#endif

namespace TinyMVC.Modules.Analytics {
    public sealed class AnalyticsParameter {
        private readonly string _parameterName;
        private readonly string _parameterStringValue;
        private readonly long _parameterLongValue;
        private readonly double _parameterDoubleValue;
        private readonly ValueType _valueType;
        
        private enum ValueType : byte {
            String,
            Long,
            Double
        }
        
        public AnalyticsParameter(string parameterName, string parameterValue) {
            _parameterName = parameterName;
            _parameterStringValue = parameterValue;
            _valueType = ValueType.String;
        }
        
        public AnalyticsParameter(string parameterName, long parameterValue) {
            _parameterName = parameterName;
            _parameterStringValue = $"{parameterValue}";
            _parameterLongValue = parameterValue;
            _valueType = ValueType.Long;
        }
        
        public AnalyticsParameter(string parameterName, double parameterValue) {
            _parameterName = parameterName;
            _parameterStringValue = $"{parameterValue}";
            _parameterDoubleValue = parameterValue;
            _valueType = ValueType.Double;
        }
        
        #if GOOGLE_FIREBASE_ANALYTICS
        public static implicit operator Parameter(AnalyticsParameter parameter) {
            switch (parameter._valueType) {
                case ValueType.String: return new Parameter(parameter._parameterName, parameter._parameterStringValue);
                case ValueType.Long: return new Parameter(parameter._parameterName, parameter._parameterLongValue);
                case ValueType.Double: return new Parameter(parameter._parameterName, parameter._parameterDoubleValue);
            }
            
            return null;
        }
        
        #endif
        
        public override string ToString() => $"{_parameterName}:{_parameterStringValue}";
    }
}