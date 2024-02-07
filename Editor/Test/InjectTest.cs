using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using TinyMVC.Boot;
using TinyMVC.Boot.Contexts;
using TinyMVC.Controllers;
using TinyMVC.Dependencies;
using TinyMVC.Views;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace TinyMVC.Test {
    public class InjectTest {
        private Assembly[] _assemblies;
        
        [SetUp]
        public void FindProjectAssemblies() {
            string[] assets = AssetDatabase.FindAssets("t:assemblyDefinitionAsset");
            List<Assembly> assemblies = new List<Assembly>();
            
            for (int assetId = 0; assetId < assets.Length; assetId++) {
                string path = AssetDatabase.GUIDToAssetPath(assets[assetId]);
                Assembly assembly;
                
                try {
                    assembly = Assembly.Load(AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(path).name);
                } catch (FileNotFoundException) {
                    continue;
                }

                assemblies.Add(assembly);
            }

            _assemblies = assemblies.ToArray();
        }
        
        [TestCase(typeof(IResolving))]
        public void InterfaceImplementation(Type interfaceType) {
            for (int assemblyId = 0; assemblyId < _assemblies.Length; assemblyId++) {
                CheckTypes(_assemblies[assemblyId].GetTypes(), interfaceType);
                Debug.Log($"{_assemblies[assemblyId].FullName} - Completed!");
            }
        }

        private void CheckTypes(Type[] types, Type interfaceType) {
            for (int typeId = 0; typeId < types.Length; typeId++) {
                CheckType(types[typeId], interfaceType);
            }
        }

        private void CheckType(Type type, Type interfaceType) {
            if (!type.IsClass) {
                return;
            }

            if (type.BaseType == typeof(ModelsContext)) {
                return;
            }
                
            if (type.BaseType == typeof(ParametersContext)) {
                return;
            }
                
            Type[] interfaces = type.GetInterfaces();
                
            bool isHaveResolvingInterface = interfaces.Contains(interfaceType);

            if (!isHaveResolvingInterface) {
                return;
            }

            bool isHaveControllerInterface = interfaces.Contains(typeof(IController));
            bool isHaveViewInterface = interfaces.Contains(typeof(IView));
                
            string message = $"{type.FullName} is have interface {interfaceType.Name} and not have {nameof(IController)} or {nameof(IView)} interface!";
            Assert.IsTrue(isHaveControllerInterface || isHaveViewInterface, message);
        }
    }
}