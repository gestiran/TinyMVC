using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using TinyMVC.ApplicationLevel.Saving.Extensions;
using TinyMVC.ApplicationLevel.Saving.VirtualFiles;
using Sirenix.Serialization;
using UnityEngine;

using SirenixSerializationUtility = Sirenix.Serialization.SerializationUtility;

namespace TinyMVC.ApplicationLevel.Saving {
    public sealed class SaveModule : IApplicationModule {
        private readonly VDirectory _main;
        private readonly string _rootPath;
        private readonly string _tempPath;

        private const string _ROOT = "UserData";
        private const string _VERSION = "V_02"; // TODO : Temp version value

        public SaveModule() {
            _rootPath = GetPath("sbf");
            _tempPath = GetPath("sbt");
            
            if (File.Exists(_rootPath)) {
                try {
                    _main = LoadRoot(_rootPath);
                } catch (Exception) {
                    try {
                        if (File.Exists(_tempPath)) {
                            _main = LoadRoot(_tempPath);
                        }
                    } catch (Exception exception) {
                        Debug.LogException(exception);
                    }
                }
            } else {
                _main = new VDirectory(_ROOT);
            }
            
            SaveProcess();
        }

    #if UNITY_EDITOR
        //[UnityEditor.MenuItem("Edit/Clear All Saves", false, 280)]
        public static void DeleteAll() {
            string path = Path.Combine(Application.persistentDataPath, _ROOT);

            if (Directory.Exists(path) == false) {
                return;
            }

            string[] files = Directory.GetFiles(path);
            
            for (int fileId = 0; fileId < files.Length; fileId++) {
                File.Delete(files[fileId]);
            }
        }
    #endif
        
        public bool HasGroup(params string[] group) => _main.HasDirectory(group);

        public bool Has(string key, params string[] group) {
            if (_main.HasDirectory(out VDirectory root, group) == false) {
                return false;
            }

            return root.HasFile(key);
        }

        public bool Has(string key) => _main.HasFile(key);

        public string[] GetGroups(params string[] group) => _main.GetAllDirectories(group);

        public string[] GetData(params string[] group) => _main.GetAllFiles(group);

        public void Save<T>(T value, string key, params string[] group) {
            _main.OpenOrCreateDirectory(group).WriteOrCreateFile(key, SerializationUtility.SerializeValue(value, DataFormat.Binary));
        }

        public bool TryLoad<T>(out T result, string key, params string[] group) {
            if (Has(key, group)) {
                result = Load<T>(key, group);
                return true;
            }

            result = default;
            return false;
        }

        public T Load<T>([Optional] T defaultValue, string key) {
            if (Has(key)) {
                return Load<T>(key);
            }

            return defaultValue;
        }

        public T Load<T>([Optional] T defaultValue, string key, params string[] group) {
            if (Has(key, group)) {
                return Load<T>(key, group);
            }

            return defaultValue;
        }

        public T Load<T>(string key) => SerializationUtility.DeserializeValue<T>(_main.GetFile(key), DataFormat.Binary);

        public T Load<T>(string key, params string[] group) {
            return SerializationUtility.DeserializeValue<T>(_main.GetDirectory(group).GetFile(key), DataFormat.Binary);
        }

        public void DeleteGroup(params string[] group) => _main.DeleteDirectory(group);

        public void Delete(string key, params string[] group) {
            if (_main.HasDirectory(out VDirectory root, group) == false) {
                return;
            }
            
            Delete(root, key);
        }

        public void Delete(string key) => Delete(_main, key);

        private void Delete(VDirectory root, string key) {
            if (root.HasFile(key) == false) {
                return;
            }
            
            root.DeleteFile(key);
        }

        private async void SaveProcess() {
            while (Application.isPlaying) {
                await SaveRoot();
                await Task.Delay(1000);
            }
        }
        
        private async Task SaveRoot() {
            byte[] bytes = SirenixSerializationUtility.SerializeValue(_main, DataFormat.Binary);
            await Task.Run(() => File.WriteAllBytesAsync(_tempPath, bytes));
            File.Delete(_rootPath);
            File.Move(_tempPath, _rootPath);
        }

        private VDirectory LoadRoot(string path) {
            using FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.None);
            return SerializationUtility.DeserializeValue<VDirectory>(fileStream, DataFormat.Binary);
        }
        
        private string GetPath(string extension) {
            string path = Path.Combine(Application.persistentDataPath, _ROOT);
            
            if (Directory.Exists(path) == false) {
                Directory.CreateDirectory(path);
            }

            return Path.Combine(path, $"{_VERSION}.{extension}");
        }
    }
}