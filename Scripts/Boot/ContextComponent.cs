using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TinyMVC.Boot.Binding;
using TinyMVC.Controllers;
using TinyMVC.Dependencies;
using TinyMVC.Views;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace TinyMVC.Boot {
    public abstract class ContextComponent : MonoBehaviour {
        [SerializeField, ListDrawerSettings(HideAddButton = true, NumberOfItemsPerPage = 5), AssetsOnly, Searchable, HideInPlayMode, Required]
        private View[] _assets;
        
        private DependencyContainer _initContainer;
        
        private List<IController> _systems;
        private List<IBinder> _binders;
        private List<IDependency> _parameters;
        
        internal void Instantiate() {
            for (int assetId = 0; assetId < _assets.Length; assetId++) {
            #if UNITY_EDITOR
                if (_assets[assetId] == null) {
                    Debug.LogError("Context contain null element!");
                    continue;
                }
            #endif
                
                _assets[assetId] = UnityObject.Instantiate(_assets[assetId]);
            }
        }
        
        internal void CreateControllersInternal(List<IController> systems) {
            _systems = systems;
            CreateControllers();
        }
        
        internal void AddComponentsViews(List<View> mainViews) => mainViews.AddRange(_assets);
        
        internal void CheckAndAdd<T>(List<T> list) {
            for (int systemId = 0; systemId < _systems.Count; systemId++) {
                if (_systems[systemId] is T controller) {
                    list.Add(controller);
                }
            }
        }
        
        internal void CreateBindersInternal(List<IBinder> binders, DependencyContainer initContainer) {
            _binders = binders;
            _initContainer = initContainer;
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
        
        protected void Add<T>() where T : IController, new() => _systems.Add(new T());
        
        protected void Add<T>(T binder, params Type[] types) where T : Binder => _binders.Add(new BinderLink(binder, types));
        
        protected void Add<T>(T binder) where T : Binder => _binders.Add(binder);
        
        protected void AddRuntime<T>(T binder) where T : Binder => ProjectBinding.Add(binder);
        
        protected T Resolve<T>(T binder) where T : Binder {
            ResolveUtility.Resolve(binder, _initContainer);
            ResolveUtility.TryApply(binder);
            return binder;
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