using System;

namespace Project.ApplicationLevel.Saving.VirtualFiles {
    [Serializable]
    public sealed class VFile {
        public string name;
        public byte[] data;

        public VFile(string name, byte[] data) {
            this.name = name;
            this.data = data;
        }
    }
}