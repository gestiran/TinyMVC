using System;

namespace TinyMVC.Modules.Saving.VirtualFiles {
    [Serializable]
    internal sealed class VFile : IDisposable {
        public byte[] data;
        public readonly string name;
        
        public VFile(string name, byte[] data) : this(name) => this.data = data;

        private VFile(string name) => this.name = name;

        public VFile Clone() {
            VFile file = new VFile(name);
            file.data = new byte[data.Length];
            Array.Copy(data, file.data, data.Length);
            return file;
        }

        public void Dispose() => data = null;
    }
}