using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using TinyMVC.Modules.Saving;
using UnityEditor;
using UnityEngine;
using Sirenix.Serialization;
using SerializationUtility = Sirenix.Serialization.SerializationUtility;

namespace TinyMVC.Editor.Modules.Saving {
    internal static class SaveModuleLog {
        public static readonly Dictionary<string, StatsData> stats;
        
        private static readonly string _persistentDataPath;
        private static readonly string _rootDirectory;
        private static readonly string _versionLabel;
        
        private const string _SAVE_STATS_FILE_NAME = "SaveStatsData";
        private const string _BASE_EXTENSION = "sbf";
        
        public sealed class StatsData {
            public string size;
            public int count;
            
            public readonly string name;
            
            public StatsData(string name, string size) {
                this.name = name;
                this.size = size;
                count = 1;
            }
        }
        
        static SaveModuleLog() {
            _persistentDataPath = Application.persistentDataPath;
            stats = Load();
            
            SaveParameters parameters = SaveParameters.LoadFromAssets();
            
            if (parameters != null) {
                _rootDirectory = parameters.rootDirectory;
                _versionLabel = parameters.versionLabel;
            } else {
                _rootDirectory = SaveParameters.ROOT_DIRECTORY;
                _versionLabel = SaveParameters.VERSION_LABEL;
            }
        }
        
        [InitializeOnLoadMethod]
        private static void Connect() => SaveService.onDataSaveEditor += AddData;
        
        private static void AddData(string directoryName) {
            if (stats.TryGetValue(directoryName, out StatsData data)) {
                data.count++;
                data.size = CalculateSize(directoryName);
            } else {
                stats.Add(directoryName, new StatsData(directoryName, CalculateSize(directoryName)));
            }
            
            Save(stats);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string CalculateSize(string name) {
            string path = GetPath(name);
            
            try {
                if (File.Exists(path)) {
                    long size = new FileInfo(GetPath(name)).Length;
                    
                    if (size < 1000) {
                        return $"<color=#00ff00>{size}b</color>";
                    }
                    
                    if (size < 1000000) {
                        return $"<color=#ffff00>{((double)size / 1000):0.0}kb</color>";
                    }
                    
                    return $"<color=#ff0000>{((double)size / 1000000):0.0}mb</color>";
                }
            } catch (Exception exception) {
                Debug.LogWarning(exception);
            }
            
            return "0";
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetPath(string name) {
            string path = Path.Combine(_persistentDataPath, $"{_rootDirectory}_{_versionLabel}");
            
            if (Directory.Exists(path) == false) {
                Directory.CreateDirectory(path);
            }
            
            return Path.Combine(path, $"{name}.{_BASE_EXTENSION}");
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Save(Dictionary<string, StatsData> data) {
            using FileStream fileStream = new FileStream(GetPath(_SAVE_STATS_FILE_NAME), FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write, 16384, FileOptions.None);
            SerializationUtility.SerializeValue(data, fileStream, DataFormat.Binary);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Dictionary<string, StatsData> Load() {
            string path = GetPath(_SAVE_STATS_FILE_NAME);
            
            if (File.Exists(path)) {
                using FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.None);
                
                return SerializationUtility.DeserializeValue<Dictionary<string, StatsData>>(fileStream, DataFormat.Binary);
            }
            
            return new Dictionary<string, StatsData>();
        }
    }
}