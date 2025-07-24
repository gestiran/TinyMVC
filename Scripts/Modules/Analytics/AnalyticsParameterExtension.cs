// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

#if GOOGLE_FIREBASE_ANALYTICS
using Firebase.Analytics;

namespace TinyMVC.Modules.Analytics {
    public static class AnalyticsParameterExtension {
        public static Parameter[] ToParameters(this AnalyticsParameter[] parameters) {
            Parameter[] result = new Parameter[parameters.Length];
            
            for (int i = 0; i < parameters.Length; i++) {
                result[i] = parameters[i].ToParameter();
            }
            
            return result;
        }
        
        private static Parameter ToParameter(this AnalyticsParameter parameter) {
            switch (parameter.type) {
                case AnalyticsParameter.ValueType.String: return new Parameter(parameter.parameterName, parameter.stringValue);
                case AnalyticsParameter.ValueType.Long: return new Parameter(parameter.parameterName, parameter.longValue);
                case AnalyticsParameter.ValueType.Double: return new Parameter(parameter.parameterName, parameter.doubleValue);
            }
            
            return null;
        }
    }
}
#endif