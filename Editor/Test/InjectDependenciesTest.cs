// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using TinyMVC.Dependencies;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace TinyMVC.Editor.Test {
    public class InjectDependenciesTest {
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
        
        [TestCase(typeof(IResolving), typeof(InjectAttribute), typeof(IDependency))]
        public void InterfaceImplementation(Type interfaceType, Type injectAttributeType, Type dependencyType) {
            for (int assemblyId = 0; assemblyId < _assemblies.Length; assemblyId++) {
                CheckTypes(_assemblies[assemblyId].GetTypes(), interfaceType, injectAttributeType, dependencyType);
                Debug.Log($"{_assemblies[assemblyId]} - Completed!");
            }
        }

        private void CheckTypes(Type[] types, Type interfaceType, Type injectAttributeType, Type dependencyType) {
            for (int typeId = 0; typeId < types.Length; typeId++) {
                CheckType(types[typeId], interfaceType, injectAttributeType, dependencyType);
            }
        }

        private void CheckType(Type type, Type interfaceType, Type injectAttributeType, Type dependencyType) {
            if (!type.IsClass) {
                return;
            }
            
            Type[] interfaces = type.GetInterfaces();
            
            bool isHaveInterface = interfaces.Contains(interfaceType);

            FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            List<FieldInfo> injectFields = GetInjectFields(fields, injectAttributeType);
            
            bool isHaveFields = injectFields.Count > 0;
                
            string message = $"{type.FullName} is have interface {interfaceType.Name} and not contain {injectAttributeType.Name} fields!";
            Assert.IsFalse(isHaveInterface && !isHaveFields, message);

            message = $"{type.FullName} is have {injectAttributeType.Name} fields {GetNames(injectFields)}and not implement interface {interfaceType.Name}!";
            Assert.IsFalse(!isHaveInterface && isHaveFields, message);

            CheckFields(type.FullName, injectFields, dependencyType);
        }

        private List<FieldInfo> GetInjectFields(FieldInfo[] fields, Type type) {
            List<FieldInfo> injectFields = new List<FieldInfo>();
            
            for (int fieldId = 0; fieldId < fields.Length; fieldId++) {
                Attribute attribute = Attribute.GetCustomAttribute(fields[fieldId], type);

                if (attribute == null) {
                    continue;
                }

                injectFields.Add(fields[fieldId]);
            }

            return injectFields;
        }

        private void CheckFields(string typeName, List<FieldInfo> injectFields, Type dependencyType) {
            for (int fieldId = 0; fieldId < injectFields.Count; fieldId++) {
                CheckField(typeName, injectFields[fieldId], dependencyType);
            }
        }
        
        private void CheckField(string typeName, FieldInfo injectField, Type dependencyType) {
            bool isHaveInterface = injectField.FieldType.GetInterfaces().Contains(dependencyType);

            string message = $"{typeName} has field {injectField.FieldType.Name} is not implement {dependencyType.Name} interface!";
            Assert.IsFalse(!isHaveInterface, message);
        }

        private string GetNames(List<FieldInfo> fields) {
            StringBuilder builder = new StringBuilder(fields.Count);

            for (int fieldId = 0; fieldId < fields.Count; fieldId++) {
                builder.Append($"[{fields[fieldId].FieldType.Name} {fields[fieldId].Name}] ");
            }
            
            return builder.ToString();
        }
    }
}
