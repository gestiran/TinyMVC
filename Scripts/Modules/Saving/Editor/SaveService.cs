// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TinyMVC.Modules.Saving.VirtualFiles;
using UnityEngine;
using UnityEngine.UIElements;

// ReSharper disable once CheckNamespace
namespace TinyMVC.Modules.Saving {
#if UNITY_EDITOR
    public static partial class SaveService {
        public static event Action onDataClearEditor;
        
        public static event Action<string> onDataSaveEditor;
        
        [UnityEditor.MenuItem("Edit/Clear All Saves", false, 280)]
        public static async void DeleteAll() {
            if (Application.isPlaying) {
                Debug.LogError("Can't clear in play mode!");
                return;
            }
            
            onDataClearEditor?.Invoke();
            
            string path;
            
            SaveParameters parameters = SaveParameters.LoadFromResources();
            
            if (parameters != null) {
                path = Path.Combine(Application.persistentDataPath, $"{parameters.rootDirectory}_{parameters.versionLabel}");
            } else {
                path = Path.Combine(Application.persistentDataPath, $"{SaveParameters.ROOT_DIRECTORY}_{SaveParameters.VERSION_LABEL}");
            }
            
            if (Directory.Exists(path)) {
                string[] files = Directory.GetFiles(path);
                
                for (int fileId = 0; fileId < files.Length; fileId++) {
                    File.Delete(files[fileId]);
                }   
            }
            
            await Task.Delay(250);
            
            _handler.Recreate();
        }
        
        public static void GetHierarchy_Editor(VisualElement element) {
            string path;
            SaveParameters parameters = SaveParameters.LoadFromResources();
            
            if (parameters != null) {
                path = Path.Combine(Application.persistentDataPath, $"{parameters.rootDirectory}_{parameters.versionLabel}");
            } else {
                path = Path.Combine(Application.persistentDataPath, $"{SaveParameters.ROOT_DIRECTORY}_{SaveParameters.VERSION_LABEL}");
            }
            
            if (Directory.Exists(path) == false) {
                element.Add(new Label("Doesn't contain files"));
                
                return;
            }
            
            string[] files = Directory.GetFiles(path);
            
            if (files.Length <= 0) {
                element.Add(new Label("Doesn't contain files"));
                
                return;
            }
            
            List<VDirectory> directories = new List<VDirectory>(files.Length);
            
            for (int fileId = 0; fileId < files.Length; fileId++) {
                if (files[fileId].Contains($".{SaveHandler.BASE_EXTENSION}") == false) {
                    continue;
                }
                
                if (_handler.TryLoadDirectory(Path.GetFileNameWithoutExtension(files[fileId]), out VDirectory directory)) {
                    directories.Add(directory);
                }
            }
            
            foreach (VDirectory directory in directories) {
                if (directory == null) {
                    continue;
                }
                
                Foldout foldout = new Foldout();
                foldout.text = $"<b>{directory.name}</b>";
                foldout.value = false;
                
                ConnectFiles_Editor(directory, foldout.contentContainer);
                ConnectDirectories_Editor(directory, foldout.contentContainer);
                
                if (foldout.childCount <= 0) {
                    foldout.text = $"{foldout.text} (Empty)";
                    foldout.value = false;
                }
                
                element.Add(foldout);
            }
        }
        
        internal static void UpdateEditor(string directoryName) => onDataSaveEditor?.Invoke(directoryName);
        
        internal static void SaveDirectories(Dictionary<string, VDirectory> directories) {
            foreach (VDirectory directory in directories.Values) {
                if (directory.isDirty == false) {
                    continue;
                }
                
                _handler.SaveDirectory(directory);
                directory.isDirty = false;
            }
        }
        
        private static void ConnectFiles_Editor(VDirectory root, VisualElement element) {
            foreach (VFile file in root.files.Values) {
                element.Add(new Label($"{file.name}.file"));
            }
        }
        
        private static void ConnectDirectories_Editor(VDirectory root, VisualElement element) {
            foreach (VDirectory directory in root.directories.Values) {
                Foldout foldout = new Foldout();
                foldout.text = $"<b>{directory.name}</b>";
                foldout.value = false;
                
                ConnectFiles_Editor(directory, foldout.contentContainer);
                ConnectDirectoriesNR_Editor(directory, foldout.contentContainer);
                
                if (foldout.childCount <= 0) {
                    foldout.text = $"{foldout.text} (Empty)";
                    foldout.value = false;
                }
                
                element.Add(foldout);
            }
        }
        
        private static void ConnectDirectoriesNR_Editor(VDirectory root, VisualElement element) {
            ConnectDirectories_Editor(root, element);
        }
    }
    
#endif
}