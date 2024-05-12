using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using JetBrains.Annotations;
using TinyMVC.ApplicationLevel.Saving.Extensions;
using TinyMVC.ApplicationLevel.Saving.VirtualFiles;
using Sirenix.Serialization;
using UnityEngine;

using SirenixSerializationUtility = Sirenix.Serialization.SerializationUtility;

namespace TinyMVC.ApplicationLevel.Saving {
    public sealed class SaveModule : IApplicationModule {
        private readonly VDirectory _main;
        private readonly Dictionary<string, VDirectory> _directories;

        private const string _MAIN_FILE_NAME = "Main";
        private const string _ROOT_DIRECTORY = "UserData";
        private const string _VERSION_LABEL = "V_02"; // TODO : Temp version value

        private const string _BASE_EXTENSION = "sbf";
        private const string _TEMP_EXTENSION = "sbt";

        private const int _CAPACITY = 16;
        private const int _DELAY_BETWEEN_SAVES = 1000;

        public SaveModule() {
            _main = LoadDirectory(_MAIN_FILE_NAME);
            _directories = new Dictionary<string, VDirectory>(_CAPACITY);
            _directories.Add(_MAIN_FILE_NAME, _main);
            
            SaveProcess();
        }

    #if UNITY_EDITOR

        [UnityEditor.MenuItem("Edit/Clear All Saves", false, 280)]
        public static void DeleteAll() {
            string path = Path.Combine(Application.persistentDataPath, $"{_ROOT_DIRECTORY}_{_VERSION_LABEL}");

            if (Directory.Exists(path) == false) {
                return;
            }

            string[] files = Directory.GetFiles(path);

            for (int fileId = 0; fileId < files.Length; fileId++) {
                File.Delete(files[fileId]);
            }
        }
        
    #endif
        
        public bool HasGroup([NotNull] params string[] group) {
            if (group.Length <= 0) {
                return false;
            }

            string directoryName = group[0];
            
            if (_directories.TryGetValue(directoryName, out VDirectory directory)) {
                return directory.HasDirectory(group);
            }
            
            if (File.Exists(GetPath(directoryName, _BASE_EXTENSION))) {
                return true;
            } 
            
            if (File.Exists(GetPath(directoryName, _TEMP_EXTENSION))) {
                return true;
            }
            
            return false;
        }

        public bool Has([NotNull] string key, [NotNull] params string[] group) {
            string directoryName = group[0];
            
            if (_directories.TryGetValue(directoryName, out VDirectory directory)) {
                if (directory.HasDirectory(out directory, group)) {
                    return directory.HasFile(key);
                }

                return false;
            }

            directory = LoadDirectory(directoryName);
            _directories.Add(directoryName, directory);
            
            if (directory.HasDirectory(out directory, group)) {
                return directory.HasFile(key);
            }

            return false;
        }

        public bool Has([NotNull] string key) => _main.HasFile(key);
        
        public string[] GetGroups([NotNull] params string[] group) {
            if (group.Length <= 0) {
                return Array.Empty<string>();
            }
            
            string directoryName = group[0];
            
            if (_directories.TryGetValue(directoryName, out VDirectory directory)) {
                return directory.GetAllDirectories(group);
            }

            directory = LoadDirectory(directoryName);
            _directories.Add(directoryName, directory);
            
            return directory.GetAllDirectories(group);
        }

        public string[] GetData([NotNull] params string[] group) {
            if (group.Length <= 0) {
                return Array.Empty<string>();
            }
            
            string directoryName = group[0];
            
            if (_directories.TryGetValue(directoryName, out VDirectory directory)) {
                if (directory.HasDirectory(out directory, group)) {
                    return directory.GetAllFiles(group);
                }

                return Array.Empty<string>();
            }

            directory = LoadDirectory(directoryName);
            _directories.Add(directoryName, directory);
            
            if (directory.HasDirectory(out directory, group)) {
                return directory.GetAllFiles(group);
            }

            return Array.Empty<string>();
        }

        public void Save<T>(T value, [NotNull] string key) => _main.WriteOrCreateFile(key, SerializationUtility.SerializeValue(value, DataFormat.Binary));

        public void Save<T>(T value, [NotNull] string key, [NotNull] params string[] group) {
            string directoryName = group[0];
            
            if (_directories.TryGetValue(directoryName, out VDirectory directory)) {
                directory.OpenOrCreateDirectory(group).WriteOrCreateFile(key, SerializationUtility.SerializeValue(value, DataFormat.Binary));
                return;
            }
            
            directory = LoadDirectory(directoryName);
            _directories.Add(directoryName, directory);
            
            directory.OpenOrCreateDirectory(group).WriteOrCreateFile(key, SerializationUtility.SerializeValue(value, DataFormat.Binary));
        }

        public bool TryLoad<T>(out T result, [NotNull] string key) {
            if (Has(key)) {
                result = LoadData<T>(key);
                return true;
            }

            result = default;
            return false;
        }

        public bool TryLoad<T>(out T result, [NotNull] string key, [NotNull] params string[] group) {
            if (Has(key, group)) {
                result = LoadData<T>(key, group);
                return true;
            }

            result = default;
            return false;
        }

        public T Load<T>([NotNull] string key) => Load(default(T), key);

        public T Load<T>(T defaultValue, [NotNull] string key) {
            if (Has(key)) {
                return LoadData<T>(key);
            }

            return defaultValue;
        }

        public T Load<T>([NotNull] string key, [NotNull] params string[] group) => Load(default(T), key, group);

        public T Load<T>(T defaultValue, [NotNull] string key, [NotNull] params string[] group) {
            if (Has(key, group)) {
                return LoadData<T>(key, group);
            }

            return defaultValue;
        }

        public void DeleteGroup([NotNull] params string[] group) {
            string directoryName = group[0];
            
            if (group.Length > 1) {
                if (_directories.TryGetValue(directoryName, out VDirectory directory)) {
                    directory.DeleteDirectory(group);
                }
            } else {
                if (_directories.ContainsKey(directoryName)) {
                    _directories.Remove(directoryName);
                }

                string path = GetPath(directoryName, _BASE_EXTENSION);
                
                if (File.Exists(path)) {
                    File.Delete(path);
                } 
            
                path = GetPath(directoryName, _TEMP_EXTENSION);
                
                if (File.Exists(path)) {
                    File.Delete(path);
                }
            }
        }

        public void Delete([NotNull] string key, [NotNull] params string[] group) {
            string directoryName = group[0];
            
            if (_directories.TryGetValue(directoryName, out VDirectory directory)) {
                if (directory.HasDirectory(out VDirectory root, group) == false) {
                    return;
                }

                Delete(root, key);
            } else {
                directory = LoadDirectory(directoryName);
                _directories.Add(directoryName, directory);
                
                if (directory.HasDirectory(out VDirectory root, group) == false) {
                    return;
                }

                Delete(root, key);
            }
        }

        public void Delete([NotNull] string key) => Delete(_main, key);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Delete(VDirectory root, string key) {
            if (root.HasFile(key) == false) {
                return;
            }

            root.DeleteFile(key);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T LoadData<T>([NotNull] string key) => SerializationUtility.DeserializeValue<T>(_main.GetFile(key), DataFormat.Binary);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T LoadData<T>([NotNull] string key, [NotNull] params string[] group) {
            string directoryName = group[0];
            
            if (_directories.TryGetValue(directoryName, out VDirectory directory)) {
                return SerializationUtility.DeserializeValue<T>(directory.GetDirectory(group).GetFile(key), DataFormat.Binary);
            }
            
            directory = LoadDirectory(directoryName);
            _directories.Add(directoryName, directory);
            
            return SerializationUtility.DeserializeValue<T>(directory.GetDirectory(group).GetFile(key), DataFormat.Binary);
        }
        
        private async void SaveProcess() {
            while (Application.isPlaying) {
                await SaveDirectory(_main);
                await SaveDirectories();
                await Task.Delay(_DELAY_BETWEEN_SAVES);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private async Task SaveDirectory(VDirectory directory) {
            if (directory.isDirty == false) {
                return;
            }
                
            byte[] bytes = SirenixSerializationUtility.SerializeValue(directory, DataFormat.Binary);
            string globalPath = GetPath(directory.name, _BASE_EXTENSION);
            string tempPath = GetPath(directory.name, _TEMP_EXTENSION);
            directory.ClearDirty();
                
            await Task.Run(() => File.WriteAllBytesAsync(tempPath, bytes));
                
            File.Delete(globalPath);
            File.Move(tempPath, globalPath);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private async Task SaveDirectories() {
            foreach (VDirectory directory in _directories.Values) {
                await SaveDirectory(directory);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private VDirectory LoadDirectory(string name) {
            string globalPath = GetPath(name, _BASE_EXTENSION);
            string tempPath = GetPath(name, _TEMP_EXTENSION);
            
            if (File.Exists(globalPath)) {
                try {
                    return LoadRoot(globalPath);
                } catch (Exception) {
                    try {
                        if (File.Exists(tempPath)) {
                            return LoadRoot(tempPath);
                        }
                    } catch (Exception exception) {
                        Debug.LogException(exception);
                        return new VDirectory(name);
                    }
                }
            } 
            
            return new VDirectory(name);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private VDirectory LoadRoot(string path) {
            using FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.None);
            return SerializationUtility.DeserializeValue<VDirectory>(fileStream, DataFormat.Binary);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string GetPath(string name, string extension) {
            string path = Path.Combine(Application.persistentDataPath, $"{_ROOT_DIRECTORY}_{_VERSION_LABEL}");

            if (Directory.Exists(path) == false) {
                Directory.CreateDirectory(path);
            }

            return Path.Combine(path, $"{name}.{extension}");
        }
    }
}