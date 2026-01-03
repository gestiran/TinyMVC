// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Collections.Generic;
using TinyMVC.Boot.Binding;
using TinyMVC.Boot.Contexts;
using TinyMVC.Controllers;
using TinyMVC.Dependencies;
using TinyMVC.Views;
using TinyReactive.Fields;
using UnityEngine;
using UnityObject = UnityEngine.Object;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Boot {
    public abstract class ContextComponent : MonoBehaviour {
    #if ODIN_INSPECTOR
        [ListDrawerSettings(NumberOfItemsPerPage = 5), AssetsOnly, Searchable, HideInPlayMode, Required]
    #endif
        [SerializeField]
        private View[] _assets;
        
        private View[] _instances;
        private List<IController> _systems;
        private List<ActionListener> _initSystemsLazy;
        private ModelsContext _models;
        private List<IDependency> _parameters;
        
        internal void Instantiate() {
            _instances = new View[_assets.Length];
            
            for (int assetId = 0; assetId < _assets.Length; assetId++) {
            #if UNITY_EDITOR
                if (_assets[assetId] == null) {
                    Debug.LogError("Context contain null element!");
                    continue;
                }
            #endif
                
                _instances[assetId] = UnityObject.Instantiate(_assets[assetId]);
            }
        }
        
        internal void CreateControllersInternal(List<IController> systems, List<ActionListener> initSystemsLazy) {
            _systems = systems;
            _initSystemsLazy = initSystemsLazy;
            CreateControllers();
        }
        
        internal void AddComponentsViews(List<View> mainViews) => mainViews.AddRange(_instances);
        
        internal void CheckAndAdd<T>(List<T> list) {
            for (int systemId = 0; systemId < _systems.Count; systemId++) {
                if (_systems[systemId] is T controller) {
                    list.Add(controller);
                }
            }
        }
        
        internal void CreateBindersInternal<T>(T context) where T : ModelsContext {
            _models = context;
            CreateBinders();
        }
        
        internal void CreateModelsInternal(List<IDependency> models) => CreateModels(models);
        
        internal void CreateParametersInternal(List<IDependency> parameters) {
            _parameters = parameters;
            CreateParameters();
        }
        
        protected virtual void CreateControllers() {
            // Empty
        }
        
        protected virtual void CreateBinders() {
            // Empty
        }
        
        protected virtual void CreateModels(List<IDependency> models) {
            // Empty
        }
        
        protected virtual void CreateParameters() {
            // Empty
        }
        
        protected void Add<T>() where T : IController, new() => _initSystemsLazy.Add(() => _systems.Add(new T()));
        
        protected void AddBinder<T>(T binder) where T : Binder {
            if (binder is IBindConditions conditions && conditions.IsNeedBinding() == false) {
                return;
            }
            
            IDependency dependency = binder.GetDependency();
            ProjectContext.data.Add(_models.key, dependency);
            _models.dependenciesBinded.Add(dependency);
        }
        
        protected void Load<T>(T dependency) where T : ScriptableObject, IDependency {
        #if UNITY_EDITOR
            
            if (dependency == null) {
                Debug.LogError($"Can't find {typeof(T).Name} parameter");
                return;
            }
            
        #endif
            
            _parameters.Add(dependency);
        }
    }
}