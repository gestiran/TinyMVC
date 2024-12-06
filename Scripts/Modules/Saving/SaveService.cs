using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TinyMVC.Modules.Saving.Extensions;
using TinyMVC.Modules.Saving.VirtualFiles;
using UnityEngine;
using Sirenix.Serialization;
using SirenixSerializationUtility = Sirenix.Serialization.SerializationUtility;

namespace TinyMVC.Modules.Saving {
    public static partial class SaveService {
        private static Dictionary<string, VDirectory> _directories;
        private static bool _isSaving;
        
        private static readonly string _persistentDataPath;
        private static readonly string _rootDirectory;
        private static readonly string _versionLabel;
        
        private const string _MAIN_FILE_NAME = "Main";
        private const string _BASE_EXTENSION = "sbf";
        private const string _TEMP_EXTENSION = "sbt";
        private const int _CAPACITY = 16;
        private const int _DELAY_BETWEEN_SAVES = 1000;
        
        static SaveService() {
            SaveParameters parameters = SaveParameters.LoadFromResources();
            
            if (parameters != null) {
                _rootDirectory = parameters.rootDirectory;
                _versionLabel = parameters.versionLabel;
            } else {
                _rootDirectory = SaveParameters.ROOT_DIRECTORY;
                _versionLabel = SaveParameters.VERSION_LABEL;
            }
            
            _persistentDataPath = Application.persistentDataPath;
            
            _directories = new Dictionary<string, VDirectory>(_CAPACITY);
            _directories.Add(_MAIN_FILE_NAME, LoadDirectory(_MAIN_FILE_NAME));
        }
        
        public static bool HasGroup(params string[] group) {
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
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsNull(string key) => key == null;
        
        public static bool Has(string key, params string[] group) {
            if (IsNull(key, group)) {
                return false;
            }
            
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
        
        public static bool Has(string key) {
            if (IsNull(key)) {
                return false;
            }
            
            return _directories[_MAIN_FILE_NAME].HasFile(key);
        }
        
        public static string[] GetGroups(params string[] group) {
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
        
        public static string[] GetData(params string[] group) {
            if (group.Length <= 0) {
                return Array.Empty<string>();
            }
            
            string directoryName = group[0];
            
            if (_directories.TryGetValue(directoryName, out VDirectory directory)) {
                return directory.GetAllFiles(group);
            }
            
            directory = LoadDirectory(directoryName);
            _directories.Add(directoryName, directory);
            
            return directory.GetAllFiles(group);
        }
        
        public static void Save<T>(T value, string key) {
            VDirectory directory = _directories[_MAIN_FILE_NAME];
            directory.semaphore.Wait();
            
            try {
                directory.WriteOrCreateFile(key, SerializationUtility.SerializeValue(value, DataFormat.Binary));
            } catch (Exception exception) {
                Debug.LogWarning(new Exception($"SaveService.Save - \"{key}\"", exception));
            } finally {
                directory.semaphore.Release();
            }
            
        #if UNITY_EDITOR
            
            if (Application.isPlaying == false) {
                SaveDirectories(_directories);
            }
            
        #endif
        }
        
        public static void Save<T>(T value, string key, params string[] group) {
            string directoryName = group[0];
            
            if (_directories.TryGetValue(directoryName, out VDirectory directory)) {
                directory.OpenOrCreateDirectory(group).WriteOrCreateFile(key, SerializationUtility.SerializeValue(value, DataFormat.Binary));
            } else {
                directory = LoadDirectory(directoryName);
                _directories.Add(directoryName, directory);
                
                directory.semaphore.Wait();
                
                try {
                    directory.OpenOrCreateDirectory(group).WriteOrCreateFile(key, SerializationUtility.SerializeValue(value, DataFormat.Binary));
                } catch (Exception exception) {
                    Debug.LogWarning(new Exception($"SaveService.Save - Sub \"{GetDebugPath(group)}\"", exception));
                } finally {
                    directory.semaphore.Release();
                }
            }
            
        #if UNITY_EDITOR
            
            if (Application.isPlaying == false) {
                SaveDirectories(_directories);
            }
            
        #endif
        }
        
        public static bool TryLoad<T>(out T result, string key) {
            if (Has(key)) {
                result = LoadData<T>(key);
                
                return true;
            }
            
            result = default;
            
            return false;
        }
        
        public static bool TryLoad<T>(out T result, string key, params string[] group) {
            if (Has(key, group)) {
                result = LoadData<T>(key, group);
                
                return true;
            }
            
            result = default;
            
            return false;
        }
        
        public static T Load<T>(string key) => Load(default(T), key);
        
        public static T Load<T>(T defaultValue, string key) {
            if (Has(key)) {
                return LoadData<T>(key);
            }
            
            return defaultValue;
        }
        
        public static T Load<T>(string key, params string[] group) => Load(default(T), key, group);
        
        public static T Load<T>(T defaultValue, string key, params string[] group) {
            if (Has(key, group)) {
                return LoadData<T>(key, group);
            }
            
            return defaultValue;
        }
        
        public static void DeleteGroup(params string[] group) {
            string directoryName = group[0];
            
            if (group.Length > 1) {
                if (_directories.TryGetValue(directoryName, out VDirectory directory)) {
                    directory.semaphore.Wait();
                    
                    try {
                        directory.DeleteDirectory(group);
                    } catch (Exception exception) {
                        Debug.LogWarning(new Exception($"SaveService.DeleteGroup - Root \"{GetDebugPath(group)}\"", exception));
                    } finally {
                        directory.semaphore.Release();
                    }
                }
            } else {
                if (_directories.ContainsKey(directoryName)) {
                    VDirectory directory = _directories[directoryName];
                    directory.semaphore.Wait();
                    
                    try {
                        _directories.Remove(directoryName);
                    } catch (Exception exception) {
                        Debug.LogWarning(new Exception($"SaveService.DeleteGroup - Sub \"{GetDebugPath(group)}\"", exception));
                    } finally {
                        directory.semaphore.Release();
                    }
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
            
        #if UNITY_EDITOR
            
            if (Application.isPlaying == false) {
                SaveDirectories(_directories);
            }
            
        #endif
        }
        
        public static void Delete(string key, params string[] group) {
            string directoryName = group[0];
            
            if (_directories.TryGetValue(directoryName, out VDirectory directory)) {
                if (directory.HasDirectory(out VDirectory root, group) == false) {
                    return;
                }
                
                if (root.HasFile(key) == false) {
                    return;
                }
                
                directory.semaphore.Wait();
                
                try {
                    root.DeleteFile(key);
                    directory.isDirty = true;
                } catch (Exception exception) {
                    Debug.LogWarning(new Exception($"SaveService.Delete - Sub \"{GetDebugPath(group)}\"\\\"{key}\"", exception));
                } finally {
                    directory.semaphore.Release();
                }
            } else {
                directory = LoadDirectory(directoryName);
                _directories.Add(directoryName, directory);
                
                if (directory.HasDirectory(out VDirectory root, group) == false) {
                    return;
                }
                
                if (root.HasFile(key) == false) {
                    return;
                }
                
                directory.semaphore.Wait();
                
                try {
                    root.DeleteFile(key);
                    directory.isDirty = true;
                } catch (Exception exception) {
                    Debug.LogWarning(new Exception($"SaveService.Delete - Root \"{GetDebugPath(group)}\"\\\"{key}\"", exception));
                } finally {
                    directory.semaphore.Release();
                }
            }
            
        #if UNITY_EDITOR
            
            if (Application.isPlaying == false) {
                SaveDirectories(_directories);
            }
            
        #endif
        }
        
        public static void Delete(string key) {
            VDirectory directory = _directories[_MAIN_FILE_NAME];
            
            if (directory.HasFile(key) == false) {
                return;
            }
            
            directory.semaphore.Wait();
            
            try {
                directory.DeleteFile(key);
                directory.isDirty = true;
            } catch (Exception exception) {
                Debug.LogWarning(new Exception($"SaveService.Delete - \"{key}\"", exception));
            } finally {
                directory.semaphore.Release();
            }
            
        #if UNITY_EDITOR
            
            if (Application.isPlaying == false) {
                SaveDirectories(_directories);
            }
            
        #endif
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T LoadData<T>(string key) {
            _directories[_MAIN_FILE_NAME].semaphore.Wait();
            
            try {
                return SerializationUtility.DeserializeValue<T>(_directories[_MAIN_FILE_NAME].GetFile(key), DataFormat.Binary);
            } catch (Exception exception) {
                Debug.LogWarning(new Exception($"SaveService.LoadData - Deserialize \"{key}\"", exception));
                return default;
            } finally {
                _directories[_MAIN_FILE_NAME].semaphore.Release();
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T LoadData<T>(string key, params string[] group) {
            string directoryName = group[0];
            
            if (_directories.TryGetValue(directoryName, out VDirectory directory)) {
                directory.semaphore.Wait();
                
                try {
                    return SerializationUtility.DeserializeValue<T>(directory.GetDirectory(group).GetFile(key), DataFormat.Binary);
                } catch (Exception exception) {
                    Debug.LogWarning(new Exception($"SaveService.LoadData - Deserialize sub \"{GetDebugPath(group)}\"\\\"{key}\"", exception));
                } finally {
                    directory.semaphore.Release();
                }
            } else {
                try {
                    directory = LoadDirectory(directoryName);
                    _directories.Add(directoryName, directory);
                    return SerializationUtility.DeserializeValue<T>(directory.GetDirectory(group).GetFile(key), DataFormat.Binary);
                } catch (Exception exception) {
                    Debug.LogWarning(new Exception($"SaveService.LoadData - Deserialize root \"{GetDebugPath(group)}\"\\\"{key}\"", exception));
                }
            }
            
            return default;
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static async void SaveProcess() {
            if (_isSaving) {
                return;
            }
            
            _isSaving = true;
            
            while (Application.isPlaying) {
                try {
                    List<Task> saveTasks = new List<Task>(_directories.Count);
                    
                    foreach (VDirectory directory in _directories.Values) {
                        if (directory.isDirty == false) {
                            continue;
                        }
                        
                        saveTasks.Add(Task.Run(() => SaveDirectory(directory)));
                        
                    #if UNITY_EDITOR
                        onDataSaveEditor?.Invoke(directory.name);
                    #endif
                    }
                    
                    if (saveTasks.Count > 0) {
                        await Task.WhenAll(saveTasks);
                    }
                } catch (Exception exception) {
                    Debug.LogWarning(new Exception("SaveService.SaveProcess", exception));
                }
                
                await Task.Delay(_DELAY_BETWEEN_SAVES);
            }
            
            _isSaving = false;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SaveDirectory(VDirectory directory) {
            directory.semaphore.Wait();
            
            try {
                string savePath = GetPath(directory.name, _BASE_EXTENSION);
                
                if (File.Exists(savePath)) {
                    string tempPath = GetPath(directory.name, _TEMP_EXTENSION);
                    
                    if (File.Exists(tempPath)) {
                        File.Delete(tempPath);
                    }
                    
                    File.Move(savePath, tempPath);
                }
                
                using FileStream fileStream = new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write, 16384, FileOptions.None);
                SirenixSerializationUtility.SerializeValue(directory.GetClone(), fileStream, DataFormat.Binary);
            } catch (Exception exception) {
                Debug.LogWarning(new Exception($"SaveService.SaveDirectory - {directory.name}", exception));
                return;
            } finally {
                directory.semaphore.Release();
            }
            
            directory.isDirty = false;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static VDirectory LoadDirectory(string name) {
            string globalPath = GetPath(name, _BASE_EXTENSION);
            
            if (File.Exists(globalPath)) {
                try {
                    return LoadRoot(globalPath);
                } catch (Exception globalException) {
                    Debug.LogWarning(new Exception($"SaveService.LoadDirectory - Load global \"{globalPath}\"", globalException));
                    
                    try {
                        File.Delete(globalPath);
                    } catch (Exception deleteGlobalException) {
                        Debug.LogWarning(new Exception($"SaveService.LoadDirectory - Delete global \"{globalPath}\"", deleteGlobalException));
                    }
                    
                    string tempPath = GetPath(name, _TEMP_EXTENSION);
                    
                    if (File.Exists(tempPath)) {
                        try {
                            return LoadRoot(tempPath);
                        } catch (Exception loadTempException) {
                            Debug.LogWarning(new Exception($"SaveService.LoadDirectory - Load temp \"{tempPath}\"", loadTempException));
                            
                            try {
                                File.Delete(tempPath);
                            } catch (Exception deleteTempException) {
                                Debug.LogWarning(new Exception($"SaveService.LoadDirectory - Delete temp \"{tempPath}\"", deleteTempException));
                            }
                        }
                    }
                }
            }
            
            return new VDirectory(name);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static VDirectory LoadRoot(string path) {
            using FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.None);
            VDirectory result = SerializationUtility.DeserializeValue<VDirectory>(fileStream, DataFormat.Binary);
            result.semaphore = VDirectory.CreateSemaphore();
            return result;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetPath(string name, string extension) {
            string path = Path.Combine(_persistentDataPath, $"{_rootDirectory}_{_versionLabel}");
            
            if (Directory.Exists(path) == false) {
                Directory.CreateDirectory(path);
            }
            
            return Path.Combine(path, $"{name}.{extension}");
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetDebugPath(string[] group) {
            StringBuilder builder = new StringBuilder(group.Length);
            
            for (int i = 0; i < group.Length; i++) {
                builder.Append(group);
                builder.Append("\\");
            }
            
            return builder.ToString();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsNull(string key, params string[] group) {
            if (IsNull(key)) {
                return true;
            }
            
            for (int groupId = 0; groupId < group.Length; groupId++) {
                if (group[groupId] == null) {
                    return true;
                }
            }
            
            return false;
        }
    }
}