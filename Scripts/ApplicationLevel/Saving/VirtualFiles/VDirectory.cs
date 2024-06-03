using System;
using System.Collections.Generic;

namespace TinyMVC.ApplicationLevel.Saving.VirtualFiles {
    [Serializable]
    internal sealed class VDirectory : IDisposable {
        [field: NonSerialized] public bool isDirty { get; private set; }

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

        public VDirectory Clone() {
            VDirectory result = new VDirectory(name);
            
            CopyDirectories(directories, result.directories);
            CopyFiles(files, result.files);
            
            return result;
        }

        private void CopyDirectories(Dictionary<string, VDirectory> source, Dictionary<string, VDirectory> destination) {
            foreach (VDirectory directory in source.Values) {
                destination.Add(directory.name, directory.Clone());
            }
        }

        private void CopyFiles(Dictionary<string, VFile> source, Dictionary<string, VFile> destination) {
            foreach (VFile file in source.Values) {
                destination.Add(file.name, file.Clone());
            }
        }

        public void Dispose() {
            foreach (VDirectory directory in directories.Values) {
                directory.Dispose();
            }

            foreach (VFile file in files.Values) {
                file.Dispose();
            }
        }
    }
}