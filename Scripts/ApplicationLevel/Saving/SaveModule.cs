#if ODIN_SERIALIZATION
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
        #if UNITY_EDITOR
        public static event Action onDataClearEditor;
        #endif

        private Dictionary<string, VDirectory> _directories;

        private readonly string _persistentDataPath;
        private readonly string _rootDirectory;
        private readonly string _versionLabel;

        private const string _MAIN_FILE_NAME = "Main";

        private const string _BASE_EXTENSION = "sbf";
        private const string _TEMP_EXTENSION = "sbt";

        private const int _CAPACITY = 16;
        private const int _DELAY_BETWEEN_SAVES = 1000;

        public SaveModule() {
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

            #if UNITY_EDITOR

            UnityEditor.EditorApplication.playModeStateChanged += PlayModeChange;

            if (Application.isPlaying) {
                SaveProcess();
            }
            #else
            SaveProcess();
            #endif
        }

        #if UNITY_EDITOR

        [UnityEditor.MenuItem("Edit/Clear All Saves", false, 280)]
        public static void DeleteAll() {
            onDataClearEditor?.Invoke();

            string path;

            SaveParameters parameters = SaveParameters.LoadFromResources();

            if (parameters != null) {
                path = Path.Combine(Application.persistentDataPath, $"{parameters.rootDirectory}_{parameters.versionLabel}");
            } else {
                path = Path.Combine(Application.persistentDataPath, $"{SaveParameters.ROOT_DIRECTORY}_{SaveParameters.VERSION_LABEL}");
            }

            if (Directory.Exists(path) == false) {
                return;
            }

            string[] files = Directory.GetFiles(path);

            for (int fileId = 0; fileId < files.Length; fileId++) {
                File.Delete(files[fileId]);
            }
        }

        private void PlayModeChange(UnityEditor.PlayModeStateChange state) {
            if (state == UnityEditor.PlayModeStateChange.ExitingPlayMode) {
                _directories = new Dictionary<string, VDirectory>(_CAPACITY);
                _directories.Add(_MAIN_FILE_NAME, LoadDirectory(_MAIN_FILE_NAME));
            } else if (state == UnityEditor.PlayModeStateChange.EnteredPlayMode) {
                SaveProcess();
            }
        }

        public void GetHierarchy_Editor(UnityEngine.UIElements.VisualElement element) {
            string path;
            SaveParameters parameters = SaveParameters.LoadFromResources();

            if (parameters != null) {
                path = Path.Combine(Application.persistentDataPath, $"{parameters.rootDirectory}_{parameters.versionLabel}");
            } else {
                path = Path.Combine(Application.persistentDataPath, $"{SaveParameters.ROOT_DIRECTORY}_{SaveParameters.VERSION_LABEL}");
            }

            if (Directory.Exists(path) == false) {
                element.Add(new UnityEngine.UIElements.Label("Doesn't contain files"));

                return;
            }

            string[] files = Directory.GetFiles(path);

            if (files.Length <= 0) {
                element.Add(new UnityEngine.UIElements.Label("Doesn't contain files"));

                return;
            }

            VDirectory[] directories = new VDirectory[files.Length];

            for (int fileId = 0; fileId < files.Length; fileId++) {
                directories[fileId] = LoadDirectory(Path.GetFileNameWithoutExtension(files[fileId]));
            }

            foreach (VDirectory directory in directories) {
                UnityEngine.UIElements.Foldout foldout = new UnityEngine.UIElements.Foldout();
                foldout.text = $"<b>{directory.name}.{_BASE_EXTENSION}</b>";
                foldout.value = true;

                ConnectFiles_Editor(directory, foldout.contentContainer);
                ConnectDirectories_Editor(directory, foldout.contentContainer);

                if (foldout.childCount <= 0) {
                    foldout.text = $"{foldout.text} (Empty)";
                    foldout.value = false;
                }

                element.Add(foldout);
            }
        }

        private void ConnectFiles_Editor(VDirectory root, UnityEngine.UIElements.VisualElement element) {
            foreach (VFile file in root.files.Values) {
                element.Add(new UnityEngine.UIElements.Label($"{file.name}.file"));
            }
        }

        private void ConnectDirectories_Editor(VDirectory root, UnityEngine.UIElements.VisualElement element) {
            foreach (VDirectory directory in root.directories.Values) {
                UnityEngine.UIElements.Foldout foldout = new UnityEngine.UIElements.Foldout();
                foldout.text = $"<b>{directory.name}</b>";
                foldout.value = true;

                ConnectFiles_Editor(directory, foldout.contentContainer);
                ConnectDirectoriesNR_Editor(directory, foldout.contentContainer);

                if (foldout.childCount <= 0) {
                    foldout.text = $"{foldout.text} (Empty)";
                    foldout.value = false;
                }

                element.Add(foldout);
            }
        }

        private void ConnectDirectoriesNR_Editor(VDirectory root, UnityEngine.UIElements.VisualElement element) { ConnectDirectories_Editor(root, element); }

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

        public bool Has([NotNull] string key) => _directories[_MAIN_FILE_NAME].HasFile(key);

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
                return directory.GetAllFiles(group);
            }

            directory = LoadDirectory(directoryName);
            _directories.Add(directoryName, directory);

            return directory.GetAllFiles(group);
        }

        public void Save<T>(T value, [NotNull] string key) {
            _directories[_MAIN_FILE_NAME].WriteOrCreateFile(key, SerializationUtility.SerializeValue(value, DataFormat.Binary));

            #if UNITY_EDITOR

            if (Application.isPlaying == false) {
                SaveDirectories(_directories);
            }

            #endif
        }

        public void Save<T>(T value, [NotNull] string key, [NotNull] params string[] group) {
            string directoryName = group[0];

            if (_directories.TryGetValue(directoryName, out VDirectory directory)) {
                directory.OpenOrCreateDirectory(group).WriteOrCreateFile(key, SerializationUtility.SerializeValue(value, DataFormat.Binary));
            } else {
                directory = LoadDirectory(directoryName);
                _directories.Add(directoryName, directory);

                directory.OpenOrCreateDirectory(group).WriteOrCreateFile(key, SerializationUtility.SerializeValue(value, DataFormat.Binary));
            }

            #if UNITY_EDITOR

            if (Application.isPlaying == false) {
                SaveDirectories(_directories);
            }

            #endif
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

            #if UNITY_EDITOR

            if (Application.isPlaying == false) {
                SaveDirectories(_directories);
            }

            #endif
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

            #if UNITY_EDITOR

            if (Application.isPlaying == false) {
                SaveDirectories(_directories);
            }

            #endif
        }

        public void Delete([NotNull] string key) {
            Delete(_directories[_MAIN_FILE_NAME], key);

            #if UNITY_EDITOR

            if (Application.isPlaying == false) {
                SaveDirectories(_directories);
            }

            #endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Delete(VDirectory root, string key) {
            if (root.HasFile(key) == false) {
                return;
            }

            root.DeleteFile(key);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T LoadData<T>([NotNull] string key) => SerializationUtility.DeserializeValue<T>(_directories[_MAIN_FILE_NAME].GetFile(key), DataFormat.Binary);

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
                await SaveDirectoriesAsync(_directories);
                await Task.Delay(_DELAY_BETWEEN_SAVES);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SaveDirectories(Dictionary<string, VDirectory> directories) {
            foreach (VDirectory directory in directories.Values) {
                if (directory.isDirty == false) {
                    continue;
                }

                SaveDirectory(directory);
                directory.ClearDirty();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private async Task SaveDirectoriesAsync(Dictionary<string, VDirectory> directories) {
            #if !UNITY_EDITOR
            UnityEngine.Scripting.GarbageCollector.GCMode = UnityEngine.Scripting.GarbageCollector.Mode.Manual;
            #endif

            foreach (VDirectory directory in directories.Values) {
                if (directory.isDirty == false) {
                    continue;
                }

                VDirectory data = directory.Clone();

                await Task.Run(() => SaveDirectory(data));
                data.Dispose();

                directory.ClearDirty();

                #if !UNITY_EDITOR
                UnityEngine.Scripting.GarbageCollector.CollectIncremental(500000);
                #endif
            }

            #if !UNITY_EDITOR
            UnityEngine.Scripting.GarbageCollector.GCMode = UnityEngine.Scripting.GarbageCollector.Mode.Enabled;
            #endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SaveDirectory(VDirectory directory) {
            string globalPath = GetPath(directory.name, _BASE_EXTENSION);
            string tempPath = GetPath(directory.name, _TEMP_EXTENSION);

            try {
                using FileStream fileStream = new FileStream(tempPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write, 16384, FileOptions.None);
                SirenixSerializationUtility.SerializeValue(directory, fileStream, DataFormat.Binary);
            } catch (Exception exception) {
                Console.WriteLine(exception);

                return;
            }

            File.Delete(globalPath);
            File.Move(tempPath, globalPath);
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
            string path = Path.Combine(_persistentDataPath, $"{_rootDirectory}_{_versionLabel}");

            if (Directory.Exists(path) == false) {
                Directory.CreateDirectory(path);
            }

            return Path.Combine(path, $"{name}.{extension}");
        }
    }
}
#endif