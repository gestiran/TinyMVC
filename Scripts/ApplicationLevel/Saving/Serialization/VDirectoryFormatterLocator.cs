using System;
using TinyMVC.ApplicationLevel.Saving.Serialization;
using Sirenix.Serialization;

[assembly: RegisterFormatterLocator(typeof (VDirectoryFormatterLocator))]

namespace TinyMVC.ApplicationLevel.Saving.Serialization {
    public sealed class VDirectoryFormatterLocator : IFormatterLocator {
        private const string _TYPE_NAME = "TinyMVC.ApplicationLevel.Saving.VirtualFiles.VDirectory";
        
        public bool TryGetFormatter(Type type, FormatterLocationStep step, ISerializationPolicy policy, bool allowWeakFallbackFormatters, out IFormatter formatter) {
            if (type.FullName == _TYPE_NAME) {
                formatter = new VDirectoryFormatter();
                return true;
            }

            formatter = null;
            return false;
        }
    }
}