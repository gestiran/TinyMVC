using System;
using System.Collections.Generic;
using Project.ApplicationLevel.Saving.VirtualFiles;

namespace Project.ApplicationLevel.Saving.Extensions {
    public static class VDirectoryExtension {
        public static void InitializeCache(this VDirectory directory) {
            directory.directoriesCache = ConvertToCache(directory.directories);
            directory.filesCache = ConvertToCache(directory.files);
        }

        public static VDirectory AddDirectory(this VDirectory directory, string name) {
            VDirectory result = new VDirectory(name);
            result.InitializeCache();

            directory.directories.Add(result);
            directory.directoriesCache.Add(name, result);

            return result;
        }

        public static VFile WriteOrCreateFile(this VDirectory directory, string name, byte[] data) {
            if (directory.filesCache.TryGetValue(name, out VFile file)) {
                file.data = data;
                return file;
            }

            VFile result = new VFile(name, data);

            directory.files.Add(result);
            directory.filesCache.Add(name, result);
                
            return result;
        }

        public static byte[] GetFile(this VDirectory directory, string name) => directory.filesCache[name].data;
        
        public static bool HasDirectory(this VDirectory directory, params string[] group) => directory.HasDirectory(out _, group);

        public static bool HasFile(this VDirectory directory, string name) => directory.filesCache.ContainsKey(name); 
        
        public static bool HasDirectory(this VDirectory directory, out VDirectory root, params string[] group) {
            root = directory;
            
            for (int groupId = 0; groupId < group.Length; groupId++) {
                if (root.directoriesCache.TryGetValue(group[groupId], out VDirectory other)) {
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
            
            for (int groupId = 0; groupId < group.Length; groupId++) {
                directory = root;
                
                if (root.directoriesCache.TryGetValue(group[groupId], out VDirectory other)) {
                    root = other;
                } else {
                    return;
                }
                
                directoryName = group[groupId];
            }
            
            directory.directoriesCache.Remove(directoryName);
            directory.directories.RemoveAt(directory.directories.FindIndex(other => other.name.Equals(directoryName)));
        }
        
        public static void DeleteFile(this VDirectory directory, string name) {
            directory.filesCache.Remove(name);
            directory.files.RemoveAt(directory.files.FindIndex(other => other.name.Equals(name)));
        }
        
        public static string[] GetAllDirectories(this VDirectory directory, params string[] group) {
            if (directory.HasDirectory(out VDirectory root, group) == false) {
                return Array.Empty<string>();
            }

            string[] result = new string[root.directories.Count];
            
            for (int i = 0; i < root.directories.Count; i++) {
                result[i] = root.directories[i].name;
            }
            
            return result;
        }
        
        public static string[] GetAllFiles(this VDirectory directory, params string[] group) {
            if (directory.HasDirectory(out VDirectory root, group) == false) {
                return Array.Empty<string>();
            }

            string[] result = new string[root.files.Count];
            
            for (int i = 0; i < root.files.Count; i++) {
                result[i] = root.files[i].name;
            }
            
            return result;
        }

        public static VDirectory GetDirectory(this VDirectory directory, params string[] group) {
            VDirectory root = directory;
            
            for (int groupId = 0; groupId < group.Length; groupId++) {
                root = root.directoriesCache[group[groupId]];
            }

            return root;
        }

        public static VDirectory OpenOrCreateDirectory(this VDirectory directory, string[] group) {
            VDirectory root = directory;
            
            for (int groupId = 0; groupId < group.Length; groupId++) {
                if (root.directoriesCache.TryGetValue(group[groupId], out VDirectory other)) {
                    root = other;
                } else {
                    root = root.AddDirectory(group[groupId]);
                }
            }

            return root;
        }

        private static Dictionary<string, VDirectory> ConvertToCache(List<VDirectory> data) {
            Dictionary<string, VDirectory> cache = new Dictionary<string, VDirectory>(data.Count);

            for (int dataId = 0; dataId < data.Count; dataId++) {
                VDirectory directory = data[dataId];
                cache.Add(directory.name, directory);
                directory.InitializeCache();
            }

            return cache;
        }

        private static Dictionary<string, VFile> ConvertToCache(List<VFile> data) {
            Dictionary<string, VFile> cache = new Dictionary<string, VFile>(data.Count);

            for (int dataId = 0; dataId < data.Count; dataId++) {
                VFile file = data[dataId];
                cache.Add(file.name, file);
            }

            return cache;
        }
    }
}