using Project.ApplicationLevel.Saving.VirtualFiles;
using Sirenix.Serialization;

namespace Project.ApplicationLevel.Saving.Serialization {
    public sealed class VFileFormatter : BaseFormatter<VFile> {
        private static readonly VFileSerializer _serializer = new VFileSerializer();
        
        protected override void DeserializeImplementation(ref VFile value, IDataReader reader) => value = _serializer.ReadValue(reader);

        protected override void SerializeImplementation(ref VFile value, IDataWriter writer) => _serializer.WriteValue(value, writer);
    }
}