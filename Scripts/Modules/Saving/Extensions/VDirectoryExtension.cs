using System;
using TinyMVC.Modules.Saving.VirtualFiles;

namespace TinyMVC.Modules.Saving.Extensions {
    internal static class VDirectoryExtension {
        public static VDirectory AddDirectory(this VDirectory directory, string name) {
            VDirectory result = new VDirectory(name);
            
            directory.directories.Add(name, result);
            directory.isDirty = true;
            
            return result;
        }
        
        public static VFile WriteOrCreateFile(this VDirectory directory, string name, byte[] data) {
            
            if (directory.files.TryGetValue(name, out VFile file)) {
                file.data = data;
                directory.isDirty = true;
                
                return file;
            }
            
            VFile result = new VFile(name, data);
            
            directory.files.Add(name, result);
            directory.isDirty = true;
            
            return result;
        }
        
        public static byte[] GetFile(this VDirectory directory, string name) => directory.files[name].data;
        
        public static bool HasDirectory(this VDirectory directory, params string[] group) => directory.HasDirectory(out _, group);
        
        public static bool HasFile(this VDirectory directory, string name) => directory.files.ContainsKey(name);
        
        public static bool HasDirectory(this VDirectory directory, out VDirectory root, params string[] group) {
            root = directory;
            
            for (int groupId = 1; groupId < group.Length; groupId++) {
                if (root.directories.TryGetValue(group[groupId], out VDirectory other)) {
                    root = other;
                } else {
                    return false;
                }
            }
            
            return true;
        }
        
        public static void DeleteDirectory(this VDirectory directory, params string[] group) {
            VDirectory root = directory;
            string directoryName = directory.name;
            
            for (int groupId = 1; groupId < group.Length; groupId++) {
                directory = root;
                
                if (root.directories.TryGetValue(group[groupId], out VDirectory other)) {
                    root.isDirty = true;
                    root = other;
                } else {
                    return;
                }
                
                directoryName = group[groupId];
            }
            
            directory.directories.Remove(directoryName);
        }
        
        public static void DeleteFile(this VDirectory directory, string name) {
            directory.files.Remove(name);
            directory.isDirty = true;
        }
        
        public static string[] GetAllDirectories(this VDirectory directory, params string[] group) {
            if (directory.HasDirectory(out VDirectory root, group) == false) {
                return Array.Empty<string>();
            }
            
            string[] result = new string[root.directories.Count];
            int i = 0;
            
            foreach (VDirectory other in root.directories.Values) {
                result[i++] = other.name;
            }
            
            return result;
        }
        
        public static string[] GetAllFiles(this VDirectory directory, params string[] group) {
            if (directory.HasDirectory(out VDirectory root, group) == false) {
                return Array.Empty<string>();
            }
            
            string[] result = new string[root.files.Count];
            int i = 0;
            
            foreach (VFile other in root.files.Values) {
                result[i++] = other.name;
            }
            
            return result;
        }
        
        public static VDirectory GetDirectory(this VDirectory directory, params string[] group) {
            VDirectory root = directory;
            
            for (int groupId = 1; groupId < group.Length; groupId++) {
                root = root.directories[group[groupId]];
            }
            
            return root;
        }
        
        public static VDirectory OpenOrCreateDirectory(this VDirectory directory, string[] group) {
            VDirectory root = directory;
            root.isDirty = true;
            
            for (int groupId = 1; groupId < group.Length; groupId++) {
                if (root.directories.TryGetValue(group[groupId], out VDirectory other)) {
                    root = other;
                } else {
                    root = root.AddDirectory(group[groupId]);
                }
            }
            
            return root;
        }
    }
}