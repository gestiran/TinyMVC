// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using System.Threading;

namespace TinyMVC.Modules.Saving.VirtualFiles {
    [Serializable]
    internal sealed class VDirectory : IEquatable<VDirectory> {
        [NonSerialized] public bool isDirty;
        [NonSerialized] public SemaphoreSlim semaphore;
        
        public readonly string name;
        public readonly Dictionary<string, VDirectory> directories;
        public readonly Dictionary<string, VFile> files;
        
        public VDirectory(string name) {
            this.name = name;
            semaphore = CreateSemaphore();
            directories = new Dictionary<string, VDirectory>();
            files = new Dictionary<string, VFile>();
        }
        
        public VDirectory(string name, int directoriesCapacity, int filesCapacity) {
            this.name = name;
            directories = new Dictionary<string, VDirectory>(directoriesCapacity);
            files = new Dictionary<string, VFile>(filesCapacity);
        }
        
        public VDirectory(string name, VDirectory[] directories, VFile[] files) {
            this.name = name;
            semaphore = CreateSemaphore();
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
        
        public static SemaphoreSlim CreateSemaphore() => new SemaphoreSlim(1, 1);
        
        public bool Equals(VDirectory other) => other != null && other.name.Equals(name);
        
        public VDirectory GetClone() {
            VDirectory clone = new VDirectory(name, directories.Count, files.Count);
            
            foreach (KeyValuePair<string, VDirectory> directory in directories) {
                clone.directories.Add(directory.Key, directory.Value.GetClone());
            }
            
            foreach (KeyValuePair<string, VFile> file in files) {
                clone.files.Add(file.Key, file.Value.GetClone());
            }
            
            return clone;
        }
    }
}