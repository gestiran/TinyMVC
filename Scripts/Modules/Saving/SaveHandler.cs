// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using TinyMVC.Modules.Saving.Extensions;
using TinyMVC.Modules.Saving.VirtualFiles;
using UnityEngine;

#if ODIN_SERIALIZATION
using Sirenix.Serialization;
#endif

namespace TinyMVC.Modules.Saving {
    internal sealed class SaveHandler {
        private Dictionary<string, VDirectory> _directories;
        private CancellationTokenSource _cancellation;
        
        private readonly string _rootDirectory;
        private readonly string _versionLabel;
        
        private static readonly string _persistentDataPath;
        
        internal const string BASE_EXTENSION = "sbf";
        private const string _TEMP_EXTENSION = "sbt";
        private const string _MAIN_FILE_NAME = "Main";
        private const int _CAPACITY = 16;
        
        private const int _DELAY_BETWEEN_SAVES = 1000;
        
        static SaveHandler() {
            _persistentDataPath = Application.persistentDataPath;
        }
        
        public SaveHandler(string rootDirectory, string versionLabel) {
            _rootDirectory = rootDirectory;
            _versionLabel = versionLabel;
            
            Recreate();
        }
        
        public bool HasGroup(params string[] group) {
            if (group.Length <= 0) {
                return false;
            }
            
            string directoryName = group[0];
            
            if (_directories.TryGetValue(directoryName, out VDirectory directory)) {
                return directory.HasDirectory(group);
            }
            
            if (File.Exists(GetPath(directoryName, BASE_EXTENSION))) {
                return true;
            }
            
            if (File.Exists(GetPath(directoryName, _TEMP_EXTENSION))) {
                return true;
            }
            
            return false;
        }
        
        public bool Has(string key, params string[] group) {
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
            
            TryLoadDirectory(directoryName, out directory);
            _directories.Add(directoryName, directory);
            
            if (directory.HasDirectory(out directory, group)) {
                return directory.HasFile(key);
            }
            
            return false;
        }
        
        public bool Has(string key) {
            if (IsNull(key)) {
                return false;
            }
            
            return _directories[_MAIN_FILE_NAME].HasFile(key);
        }
        
        public string[] GetGroups(params string[] group) {
            if (group.Length <= 0) {
                return Array.Empty<string>();
            }
            
            string directoryName = group[0];
            
            if (_directories.TryGetValue(directoryName, out VDirectory directory)) {
                return directory.GetAllDirectories(group);
            }
            
            TryLoadDirectory(directoryName, out directory);
            _directories.Add(directoryName, directory);
            
            return directory.GetAllDirectories(group);
        }
        
        public string[] GetData(params string[] group) {
            if (group.Length <= 0) {
                return Array.Empty<string>();
            }
            
            string directoryName = group[0];
            
            if (_directories.TryGetValue(directoryName, out VDirectory directory)) {
                return directory.GetAllFiles(group);
            }
            
            TryLoadDirectory(directoryName, out directory);
            _directories.Add(directoryName, directory);
            
            return directory.GetAllFiles(group);
        }
        
        public void Save<T>(T value, string key) {
            VDirectory directory = _directories[_MAIN_FILE_NAME];
            directory.semaphore.Wait();
            
            try {
                if (directory.WriteOrCreateFile(key, SerializationUtility.SerializeValue(value, DataFormat.Binary))) {
                    directory.isDirty = true;
                }
            } catch (Exception exception) {
                Debug.LogWarning(new Exception($"SaveService.Save - \"{key}\"", exception));
            } finally {
                directory.semaphore.Release();
            }
            
        #if UNITY_EDITOR
            
            if (Application.isPlaying == false) {
                SaveService.SaveDirectories(_directories);
            }
            
        #endif
        }
        
        public void Save<T>(T value, string key, params string[] group) {
            string directoryName = group[0];
            
            if (_directories.TryGetValue(directoryName, out VDirectory directory)) {
                if (directory.OpenOrCreateDirectory(group).WriteOrCreateFile(key, SerializationUtility.SerializeValue(value, DataFormat.Binary))) {
                    directory.isDirty = true;
                }
            } else {
                TryLoadDirectory(directoryName, out directory);
                _directories.Add(directoryName, directory);
                
                directory.semaphore.Wait();
                
                try {
                    if (directory.OpenOrCreateDirectory(group).WriteOrCreateFile(key, SerializationUtility.SerializeValue(value, DataFormat.Binary))) {
                        directory.isDirty = true;
                    }
                } catch (Exception exception) {
                    Debug.LogWarning(new Exception($"SaveService.Save - Sub \"{GetDebugPath(group)}\"", exception));
                } finally {
                    directory.semaphore.Release();
                }
            }
            
        #if UNITY_EDITOR
            
            if (Application.isPlaying == false) {
                SaveService.SaveDirectories(_directories);
            }
            
        #endif
        }
        
        public bool TryLoad<T>(out T result, string key) {
            if (Has(key)) {
                result = LoadData<T>(key);
                
                return true;
            }
            
            result = default;
            
            return false;
        }
        
        public bool TryLoad<T>(out T result, string key, params string[] group) {
            if (Has(key, group)) {
                result = LoadData<T>(key, group);
                
                return true;
            }
            
            result = default;
            
            return false;
        }
        
        public T Load<T>(T defaultValue, string key) {
            if (Has(key)) {
                return LoadData<T>(key);
            }
            
            return defaultValue;
        }
        
        public T Load<T>(T defaultValue, string key, params string[] group) {
            if (Has(key, group)) {
                return LoadData<T>(key, group);
            }
            
            return defaultValue;
        }
        
        public void DeleteGroup(params string[] group) {
            string directoryName = group[0];
            
            if (group.Length > 1) {
                if (_directories.TryGetValue(directoryName, out VDirectory directory)) {
                    directory.semaphore.Wait();
                    
                    try {
                        if (directory.DeleteDirectory(group)) {
                            directory.isDirty = true;
                        }
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
                
                string path = GetPath(directoryName, BASE_EXTENSION);
                
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
                SaveService.SaveDirectories(_directories);
            }
            
        #endif
        }
        
        public void Delete(string key, params string[] group) {
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
                    if (root.DeleteFile(key)) {
                        directory.isDirty = true;
                    }
                } catch (Exception exception) {
                    Debug.LogWarning(new Exception($"SaveService.Delete - Sub \"{GetDebugPath(group)}\"\\\"{key}\"", exception));
                } finally {
                    directory.semaphore.Release();
                }
            } else {
                TryLoadDirectory(directoryName, out directory);
                _directories.Add(directoryName, directory);
                
                if (directory.HasDirectory(out VDirectory root, group) == false) {
                    return;
                }
                
                if (root.HasFile(key) == false) {
                    return;
                }
                
                directory.semaphore.Wait();
                
                try {
                    if (root.DeleteFile(key)) {
                        directory.isDirty = true;
                    }
                } catch (Exception exception) {
                    Debug.LogWarning(new Exception($"SaveService.Delete - Root \"{GetDebugPath(group)}\"\\\"{key}\"", exception));
                } finally {
                    directory.semaphore.Release();
                }
            }
            
        #if UNITY_EDITOR
            
            if (Application.isPlaying == false) {
                SaveService.SaveDirectories(_directories);
            }
            
        #endif
        }
        
        public void Delete(string key) {
            VDirectory directory = _directories[_MAIN_FILE_NAME];
            
            if (directory.HasFile(key) == false) {
                return;
            }
            
            directory.semaphore.Wait();
            
            try {
                if (directory.DeleteFile(key)) {
                    directory.isDirty = true;
                }
            } catch (Exception exception) {
                Debug.LogWarning(new Exception($"SaveService.Delete - \"{key}\"", exception));
            } finally {
                directory.semaphore.Release();
            }
            
        #if UNITY_EDITOR
            
            if (Application.isPlaying == false) {
                SaveService.SaveDirectories(_directories);
            }
            
        #endif
        }
        
        internal void Recreate() {
            _directories = new Dictionary<string, VDirectory>(_CAPACITY);
            TryLoadDirectory(_MAIN_FILE_NAME, out VDirectory directory);
            _directories.Add(_MAIN_FILE_NAME, directory);
        }
        
        internal void Stop() {
            if (_cancellation != null) {
                _cancellation.Cancel();
                _cancellation.Dispose();
                _cancellation = null;
            }
        }
        
        internal void Start() {
            Stop();
            _cancellation = new CancellationTokenSource();
            SaveProcess(_cancellation.Token).Forget();
        }
        
        private T LoadData<T>(string key) {
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
        
        private T LoadData<T>(string key, params string[] group) {
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
                    T value;
                    
                    if (TryLoadDirectory(directoryName, out directory)) {
                        value = SerializationUtility.DeserializeValue<T>(directory.GetDirectory(group).GetFile(key), DataFormat.Binary);
                    } else {
                        value = default;
                    }
                    
                    _directories.Add(directoryName, directory);
                    return value;
                } catch (Exception exception) {
                    Debug.LogWarning(new Exception($"SaveService.LoadData - Deserialize root \"{GetDebugPath(group)}\"\\\"{key}\"", exception));
                }
            }
            
            return default;
        }
        
        internal void SaveDirectory(VDirectory directory) {
            directory.semaphore.Wait();
            
            try {
                string savePath = GetPath(directory.name, BASE_EXTENSION);
                
                if (File.Exists(savePath)) {
                    string tempPath = GetPath(directory.name, _TEMP_EXTENSION);
                    
                    if (File.Exists(tempPath)) {
                        File.Delete(tempPath);
                    }
                    
                    File.Move(savePath, tempPath);
                }
                
                using FileStream fileStream = new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write, 16384, FileOptions.None);
                SerializationUtility.SerializeValue(directory.GetClone(), fileStream, DataFormat.Binary);
            } catch (Exception exception) {
                Debug.LogWarning(new Exception($"SaveService.SaveDirectory - {directory.name}", exception));
                return;
            } finally {
                directory.semaphore.Release();
            }
            
            directory.isDirty = false;
        }
        
        internal bool TryLoadDirectory(string name, out VDirectory directory) {
            string globalPath = GetPath(name, BASE_EXTENSION);
            
            if (File.Exists(globalPath)) {
                try {
                    directory = LoadRoot(globalPath);
                    return true;
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
                            directory = LoadRoot(tempPath);
                            return true;
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
            
            directory = new VDirectory(name);
            return false;
        }
        
        private VDirectory LoadRoot(string path) {
            using FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.None);
            VDirectory result = SerializationUtility.DeserializeValue<VDirectory>(fileStream, DataFormat.Binary);
            result.semaphore = VDirectory.CreateSemaphore();
            return result;
        }
        
        private string GetPath(string name, string extension) {
            string path = Path.Combine(_persistentDataPath, $"{_rootDirectory}_{_versionLabel}");
            
            if (Directory.Exists(path) == false) {
                Directory.CreateDirectory(path);
            }
            
            return Path.Combine(path, $"{name}.{extension}");
        }
        
        private string GetDebugPath(string[] group) {
            StringBuilder builder = new StringBuilder(group.Length);
            
            for (int i = 0; i < group.Length; i++) {
                builder.Append(group);
                builder.Append("\\");
            }
            
            return builder.ToString();
        }
        
        private bool IsNull(string key, params string[] group) {
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
        
        private bool IsNull(string key) => key == null;
        
        // ReSharper disable once FunctionNeverReturns
        private async UniTask SaveProcess(CancellationToken cancellation) {
            while (true) {
                try {
                    List<UniTask> saveTasks = new List<UniTask>(_directories.Count);
                    
                    foreach (VDirectory directory in _directories.Values) {
                        if (directory.isDirty == false) {
                            continue;
                        }
                        
                        saveTasks.Add(UniTask.RunOnThreadPool(() => SaveDirectory(directory), true, cancellation));
                        
                    #if UNITY_EDITOR
                        SaveService.UpdateEditor(directory.name);
                    #endif
                    }
                    
                    if (saveTasks.Count > 0) {
                        await UniTask.WhenAll(saveTasks);
                    }
                } catch (Exception exception) {
                    Debug.LogWarning(new Exception("SaveService.SaveProcess", exception));
                }
                
                await UniTask.Delay(_DELAY_BETWEEN_SAVES, true, PlayerLoopTiming.LastTimeUpdate, cancellation);
            }
        }
    }
}