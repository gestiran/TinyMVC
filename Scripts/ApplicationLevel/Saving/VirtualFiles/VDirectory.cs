using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.ApplicationLevel.Saving.VirtualFiles {
    [Serializable]
    public sealed class VDirectory {
        public string name;
            
        [SerializeField] internal List<VDirectory> directories;
        [SerializeField] internal List<VFile> files;

        [NonSerialized] internal Dictionary<string, VDirectory> directoriesCache;
        [NonSerialized] internal Dictionary<string, VFile> filesCache;
        
        public VDirectory(string name) {
            this.name = name;
            directories = new List<VDirectory>();
            files = new List<VFile>();
        }
    }
}