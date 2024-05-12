namespace TinyMVC.ApplicationLevel.Saving.VirtualFiles {
    internal sealed class VFile {
        public byte[] data;
        
        public readonly string name;

        public VFile(string name, byte[] data) {
            this.name = name;
            this.data = data;
        }
    }
}