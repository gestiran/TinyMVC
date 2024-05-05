using TinyMVC.ApplicationLevel.Saving.VirtualFiles;
using Sirenix.Serialization;

namespace TinyMVC.ApplicationLevel.Saving.Serialization {
    public sealed class VDirectoryFormatter : BaseFormatter<VDirectory> {
        private static readonly VDirectorySerializer _serializer = new VDirectorySerializer();
        
        protected override void DeserializeImplementation(ref VDirectory value, IDataReader reader) => value = _serializer.ReadValue(reader);

        protected override void SerializeImplementation(ref VDirectory value, IDataWriter writer) => _serializer.WriteValue(value, writer);
    }
}