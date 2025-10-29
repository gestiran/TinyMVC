#if !ODIN_SERIALIZATION
    
    using System.IO;
    
    namespace TinyMVC.Modules.Saving {
        public static class SerializationUtility {
            public static byte[] SerializeValue(object value, DataFormat dataFormat) => null;
            
            public static void SerializeValue(object value, FileStream stream, DataFormat dataFormat) { }
            
            public static T DeserializeValue<T>(byte[] data, DataFormat dataFormat) => default;
            
            public static T DeserializeValue<T>(FileStream stream, DataFormat dataFormat) => default;
        }
        
        public enum DataFormat {
            Binary
        }
    }
    
#endif