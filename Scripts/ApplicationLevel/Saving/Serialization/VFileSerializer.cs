using System;
using System.Text;
using Project.ApplicationLevel.Saving.VirtualFiles;
using Sirenix.Serialization;

namespace Project.ApplicationLevel.Saving.Serialization {
    public sealed class VFileSerializer : Serializer<VFile> {
        public override VFile ReadValue(IDataReader reader) {
            reader.ReadPrimitiveArray(out byte[] data);

            int nameSize = BitConverter.ToInt32(data, 0);
            int dataSize = data.Length - (nameSize + 4);

            byte[] result = new byte[dataSize];
            Array.Copy(data, nameSize + 4, result, 0, dataSize);
            
            return new VFile(Encoding.Unicode.GetString(data, 4, nameSize), result);
        }

        public override void WriteValue(string name, VFile value, IDataWriter writer) {
            byte[] nameBytes = Encoding.Unicode.GetBytes(value.name);
            byte[] nameSize = BitConverter.GetBytes(nameBytes.Length);

            byte[] result = new byte[nameSize.Length + nameBytes.Length + value.data.Length];

            Array.Copy(nameSize, 0, result, 0, nameSize.Length);
            Array.Copy(nameBytes, 0, result, nameSize.Length, nameBytes.Length);
            Array.Copy(value.data, 0, result, nameSize.Length + nameBytes.Length, value.data.Length);
            
            writer.WritePrimitiveArray(result);
        }
    }
}