using System;
using Project.ApplicationLevel.Saving.Serialization;
using Sirenix.Serialization;

[assembly: RegisterFormatterLocator(typeof (VFileFormatterLocator))]

namespace Project.ApplicationLevel.Saving.Serialization {
    public sealed class VFileFormatterLocator : IFormatterLocator {
        public bool TryGetFormatter(Type type, FormatterLocationStep step, ISerializationPolicy policy, bool allowWeakFallbackFormatters, out IFormatter formatter) {
            if (type.FullName == "Project.ApplicationLevel.Saving.VirtualFiles.VFile") {
                formatter = new VFileFormatter();
                return true;
            }

            formatter = null;
            return false;
        }
    }
}