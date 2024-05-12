using System;
using System.Collections.Generic;
using System.Text;
using TinyMVC.ApplicationLevel.Saving.VirtualFiles;
using Sirenix.Serialization;

namespace TinyMVC.ApplicationLevel.Saving.Serialization {
    internal sealed class VDirectorySerializer : Serializer<VDirectory> {
        public override VDirectory ReadValue(IDataReader reader) {
            reader.ReadPrimitiveArray(out byte[] data);

            int startIndex = 0;
            
            int nameSize = BitConverter.ToInt32(data, startIndex);
            startIndex += 4;
            
            string name = Encoding.Unicode.GetString(data, startIndex, nameSize);
            startIndex += nameSize;
            
            int directoriesSize = BitConverter.ToInt32(data, startIndex);
            startIndex += 4;
            
            byte[] directoriesBytes = new byte[directoriesSize];
            Array.Copy(data, startIndex, directoriesBytes, 0, directoriesBytes.Length);
            VDirectory[] directories = SerializationUtility.DeserializeValue<VDirectory[]>(directoriesBytes, DataFormat.Binary);
            startIndex += directoriesSize;
            
            int filesSize = BitConverter.ToInt32(data, startIndex);
            startIndex += 4;
            
            byte[] filesBytes = new byte[filesSize];
            Array.Copy(data, startIndex, filesBytes, 0, filesBytes.Length);
            VFile[] files = SerializationUtility.DeserializeValue<VFile[]>(filesBytes, DataFormat.Binary);
            
            return new VDirectory(name, directories, files);
        }

        public override void WriteValue(string name, VDirectory value, IDataWriter writer) {
            byte[] nameBytes = Encoding.Unicode.GetBytes(value.name);
            byte[] nameSize = BitConverter.GetBytes(nameBytes.Length);

            VDirectory[] directories = GetValuesArray(value.directories);
            VFile[] files = GetValuesArray(value.files);

            byte[] directoriesBytes = SerializationUtility.SerializeValue(directories, DataFormat.Binary);
            byte[] directoriesSize = BitConverter.GetBytes(directoriesBytes.Length);
            byte[] filesBytes = SerializationUtility.SerializeValue(files, DataFormat.Binary);
            byte[] filesSize = BitConverter.GetBytes(filesBytes.Length);
            
            int size = 0;
            
            size += nameBytes.Length;
            size += nameSize.Length;
            
            size += directoriesBytes.Length;
            size += directoriesSize.Length;
            
            size += filesBytes.Length;
            size += filesSize.Length;
            
            byte[] result = new byte[size];
            
            size = 0;
            
            Array.Copy(nameSize, 0, result, size, nameSize.Length);
            size += nameSize.Length;
            
            Array.Copy(nameBytes, 0, result, size, nameBytes.Length);
            size += nameBytes.Length;
            
            Array.Copy(directoriesSize, 0, result, size, directoriesSize.Length);
            size += directoriesSize.Length;
            
            Array.Copy(directoriesBytes, 0, result, size, directoriesBytes.Length);
            size += directoriesBytes.Length;
            
            Array.Copy(filesSize, 0, result, size, filesSize.Length);
            size += filesSize.Length;
            
            Array.Copy(filesBytes, 0, result, size, filesBytes.Length);
            
            writer.WritePrimitiveArray(result);
        }

        private T[] GetValuesArray<T>(Dictionary<string, T> directories) {
            T[] result = new T[directories.Count];
            int i = 0;
            
            foreach (T other in directories.Values) {
                result[i++] = other;
            }

            return result;
        }
    }
}