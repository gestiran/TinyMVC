using System.Collections.Generic;

namespace TinyMVC.ApplicationLevel.Saving.VirtualFiles {
    public sealed class VDirectory {
        public bool isDirty { get; private set; }

        public readonly string name;
        public readonly Dictionary<string, VDirectory> directories;
        public readonly Dictionary<string, VFile> files;
        
        public VDirectory(string name) {
            this.name = name;
            directories = new Dictionary<string, VDirectory>();
            files = new Dictionary<string, VFile>();
        }
        
        public VDirectory(string name, VDirectory[] directories, VFile[] files) {
            this.name = name;
            this.directories = new Dictionary<string, VDirectory>(directories.Length);
            
            for (int i = 0; i < directories.Length; i++) {
                VDirectory directory = directories[i];
                this.directories.Add(directory.name, directory);
            }
            
            this.files = new Dictionary<string, VFile>(files.Length);
            
            for (int i = 0; i < files.Length; i++) {
                VFile file = files[i];
                this.files.Add(file.name, file);
            }
        }

        public void SetDirty() => isDirty = true;

        public void ClearDirty() => isDirty = false;
    }
}