using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Sirenix.Serialization;
using UnityEngine;

using SirenixSerializationUtility = Sirenix.Serialization.SerializationUtility;

namespace Project.Utilities {
    public static class SaveUtility {
        private static readonly string _pathToVersion;

        private const string _ROOT = "UserData";
        private const string _VERSION = "V_02"; // TODO : Temp version value
        private const string _EXTENSION = "sbf";
        
        private static readonly ConcurrentQueue<Action> _commands;

        static SaveUtility() {
            _commands = new ConcurrentQueue<Action>();

            Task.Run(
                () => {
                    while (true) {
                        while (_commands.TryDequeue(out Action command)) {
                            try {
                                command();
                            } catch (Exception ex) {
                                Debug.LogException(ex);
                                return;
                            }
                        }

                        Thread.Sleep(16);
                    }
                }
            );

            _pathToVersion = Path.Combine(Application.persistentDataPath, _ROOT, _VERSION);

            if (Directory.Exists(_pathToVersion)) {
                return;
            }

            Directory.CreateDirectory(_pathToVersion);
        }

        public static bool HasGroup(params string[] group) => Directory.Exists(Path.Combine(_pathToVersion, Path.Combine(group)));

        public static bool Has(string key, params string[] group) => Has(Path.Combine(Path.Combine(group), key));

        public static bool Has(string key) => File.Exists(Path.Combine(_pathToVersion, $"{key}.{_EXTENSION}"));

        public static string[] GetDataWithOutExtension(params string[] group) {
            if (!HasGroup(group)) {
                return Array.Empty<string>();
            }

            string[] result = Directory.GetFiles(Path.Combine(_pathToVersion, Path.Combine(group)));

            for (int pathId = 0; pathId < result.Length; pathId++) {
                string name = Path.GetFileName(result[pathId]);
                result[pathId] = name.Remove(name.Length - _EXTENSION.Length - 1);
            }

            return result;
        }

        public static string[] GetGroups(params string[] group) {
            if (!HasGroup(group)) {
                return Array.Empty<string>();
            }

            string[] result = Directory.GetDirectories(Path.Combine(_pathToVersion, Path.Combine(group)));

            for (int pathId = 0; pathId < result.Length; pathId++) {
                result[pathId] = Path.GetFileName(result[pathId]);
            }

            return result;
        }

        public static string[] GetData(params string[] group) {
            if (!HasGroup(group)) {
                return Array.Empty<string>();
            }

            string[] result = Directory.GetFiles(Path.Combine(_pathToVersion, Path.Combine(group)));

            for (int pathId = 0; pathId < result.Length; pathId++) {
                result[pathId] = Path.GetFileName(result[pathId]);
            }

            return result;
        }

        public static void Save<T>(T value, string key, params string[] group) => _commands.Enqueue(() => SaveCommand(value, key, group));

        public static bool TryLoad<T>(out T result, string key, params string[] group) {
            if (Has(key, group)) {
                result = Load<T>(key, group);

                return true;
            }

            result = default;

            return false;
        }

        public static T Load<T>([Optional] T defaultValue, string key) {
            if (Has(key)) {
                return Load<T>(key);
            }

            return defaultValue;
        }

        public static T Load<T>([Optional] T defaultValue, string key, params string[] group) {
            if (Has(key, group)) {
                return Load<T>(key, group);
            }

            return defaultValue;
        }

        public static T Load<T>(string key) => LoadFile<T>(key);

        public static T Load<T>(string key, params string[] group) => LoadFile<T>(Path.Combine(Path.Combine(group), key));

        public static void DeleteGroup(params string[] group) => _commands.Enqueue(() => DeleteGroupCommand(group));

        public static void Delete(string key, params string[] group) => Delete(Path.Combine(Path.Combine(group), key));

        public static void Delete(string key) => _commands.Enqueue(() => DeleteCommand(key));

        private static void SaveCommand<T>(T value, string key, string[] group) {
            string pathToGroup = Path.Combine(group);
            string path = Path.Combine(_pathToVersion, pathToGroup);

            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }

            SaveFile(value, Path.Combine(pathToGroup, key));
        }

        private static void DeleteGroupCommand(string[] group) {
            string path = Path.Combine(_pathToVersion, Path.Combine(group));

            if (!Directory.Exists(path)) {
                return;
            }

            Directory.Delete(path, true);
        }

        private static void DeleteCommand(string key) {
            string path = Path.Combine(_pathToVersion, $"{key}.{_EXTENSION}");
            
            if (!File.Exists(path)) {
                return;
            }

            File.Delete(path);
        }

    #if UNITY_EDITOR
        [UnityEditor.MenuItem("Edit/Clear All Saves", false, 280)]
    #endif
        public static void DeleteAll() {
            string path = Path.Combine(Application.persistentDataPath, _ROOT);

            if (!Directory.Exists(path)) {
                return;
            }

            Directory.Delete(path, true);

        #if UNITY_EDITOR
            Directory.CreateDirectory(path);
        #endif
        }

        private static void SaveFile<T>(T value, string path) {
            path = Path.Combine(_pathToVersion, $"{path}.{_EXTENSION}");
            using FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read, 512, FileOptions.None);
            SirenixSerializationUtility.SerializeValue(value, fileStream, DataFormat.Binary);
        }

        private static T LoadFile<T>(string path) {
            path = Path.Combine(_pathToVersion, $"{path}.{_EXTENSION}");
            using FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 512, FileOptions.None);
            return SirenixSerializationUtility.DeserializeValue<T>(fileStream, DataFormat.Binary);
        }
    }
}