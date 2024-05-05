namespace TinyMVC.ApplicationLevel.Saving.VirtualFiles {
    public sealed class VFile {
        public byte[] data;
        
        public readonly string name;

        public VFile(string name, byte[] data) {
            this.name = name;
            this.data = data;
        }
    }
}