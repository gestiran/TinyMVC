// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Collections.Generic;
using TinyMVC.Boot.Contexts;
using TinyMVC.Controllers;
using TinyMVC.Dependencies;
using TinyMVC.Loop;
using TinyMVC.Views;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using TinyReactive;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Boot {
    [DisallowMultipleComponent]
    public abstract class SceneContext<TViews> : SceneContext where TViews : ViewsContext {
        [field: SerializeField]
        public new TViews views { get; private set; }
        
        internal override ViewsContext viewsInternal { get => views; set => views = value as TViews; }
        
        private const int _DEPENDENCIES_CAPACITY = 64;
        
        internal override void Create() {
            unload = new UnloadPool();
            
            controllers = CreateControllers();
            models = CreateModels();
            parameters = CreateParameters();
            
            views.Instantiate();
            InstantiateComponents();
            
            controllers.CreateControllers();
            CreateComponentsControllers(controllers.systems);
            
            views.CreateViews();
            AddComponentsViews(views.mainViews);
            
            if (this is IGlobalContext) {
                views.ApplyDontDestroyOnLoad();
                DontDestroyOnLoad(this);
            }
        }
        
        internal override async UniTask InitAsync() {
            await views.InitAsync();
            
            await Resolve();
            
            await controllers.BeginPlay();
            await views.BeginPlay();
            
            controllers.CheckAndAdd(fixedTicks);
            controllers.CheckAndAdd(ticks);
            controllers.CheckAndAdd(lateTicks);
            
            views.CheckAndAdd(fixedTicks);
            views.CheckAndAdd(ticks);
            views.CheckAndAdd(lateTicks);
        }
        
        internal override void Unload() {
            controllers.Unload();
            views.Unload();
            models.Unload();
        }
        
        protected abstract ControllersContext CreateControllers();
        
        protected abstract ModelsContext CreateModels();
        
        protected abstract ParametersContext CreateParameters();
        
        private async UniTask Resolve() {
            List<IDependency> dependenciesParameters = new List<IDependency>(_DEPENDENCIES_CAPACITY);
            List<IDependency> dependenciesViews = new List<IDependency>(_DEPENDENCIES_CAPACITY);
            
            parameters.Init();
            CreateParametersComponents(parameters.all);
            
            parameters.AddDependencies(dependenciesParameters);
            
            ProjectContext.data.Add(key, dependenciesParameters);
            DependencyContainer tempContainer = new DependencyContainer(dependenciesParameters);
            ProjectContext.data.tempContainer = tempContainer;
            
            views.GetDependencies(dependenciesViews);
            
            ProjectContext.data.viewsContainer = new DependencyContainer(dependenciesViews);
            
            models.CreateBinders(key);
            CreateBindersComponents(models);
            
            List<IDependency> runtimeDependencies = new List<IDependency>(_DEPENDENCIES_CAPACITY);
            
            runtimeDependencies.AddRange(models.dependenciesBinded);
            
            tempContainer = new DependencyContainer(runtimeDependencies);
            ProjectContext.data.tempContainer = tempContainer;
            ResolveUtility.TryApply(models);
            
            models.Create();
            CreateModelsComponents(models.dependencies);
            ProjectContext.data.Add(key, models.dependencies);
            
            controllers.Init();
            
            await controllers.InitAsync();
            
            ResolveUtility.TryApply(controllers.systems);
            ResolveUtility.TryApply(views.mainViews);
        }
        
        private void InstantiateComponents() {
            for (int componentId = 0; componentId < components.Length; componentId++) {
                components[componentId].Instantiate();
            }
        }
        
        private void CreateComponentsControllers(List<IController> systems) {
            for (int componentId = 0; componentId < components.Length; componentId++) {
                components[componentId].CreateControllersInternal(systems);
            }
        }
        
        private void AddComponentsViews(List<View> mainViews) {
            for (int componentId = 0; componentId < components.Length; componentId++) {
                components[componentId].AddComponentsViews(mainViews);
            }
        }
        
        private void CreateParametersComponents(List<IDependency> dependencies) {
            for (int componentId = 0; componentId < components.Length; componentId++) {
                components[componentId].CreateParametersInternal(dependencies);
            }
        }
        
        private void CreateBindersComponents<T>(T context) where T : ModelsContext {
            for (int componentId = 0; componentId < components.Length; componentId++) {
                components[componentId].CreateBindersInternal(context);
            }
        }
        
        private void CreateModelsComponents(List<IDependency> dependencies) {
            for (int componentId = 0; componentId < components.Length; componentId++) {
                components[componentId].CreateModelsInternal(dependencies);
            }
        }
        
        internal override void Connect(View view) => views.Connect(view, ConnectLoop);
        
        internal override void Connect<T1, T2>(T2 system, T1 controller) => controllers.Connect(system, controller, ConnectLoop);
        
        internal override void Disconnect(View view) => views.Disconnect(view, DisconnectLoop);
        
        internal override void Disconnect<T1, T2>(T2 system, T1 controller) => controllers.Disconnect(system, controller, DisconnectLoop);
        
    #if UNITY_EDITOR
    #if ODIN_INSPECTOR
        [Button("Generate"), PropertyOrder(20), ShowIn(PrefabKind.InstanceInScene), HideInPlayMode]
    #endif
        public override void Reset() {
            if (views != null) {
                views.Reset();
            }
            
            base.Reset();
        }
        
    #endif
    }
    
    [DefaultExecutionOrder(-50)]
    public abstract class SceneContext : MonoBehaviour, IEquatable<SceneContext> {
        public string key { get; private set; }
        
        public ViewsContext views { get => viewsInternal; internal set => viewsInternal = value; }
        
        internal virtual ViewsContext viewsInternal { get; set; }
        
        internal List<IFixedTick> fixedTicks { get; private set; }
        internal List<ITick> ticks { get; private set; }
        internal List<ILateTick> lateTicks { get; private set; }
        
    #if ODIN_INSPECTOR
        [ShowInInspector, HideLabel, HideReferenceObjectPicker, HideDuplicateReferenceBox, InlineProperty, HideInEditorMode]
    #endif
        internal ControllersContext controllers;
        
    #if ODIN_INSPECTOR
        [PropertyOrder(10), InlineEditor(InlineEditorObjectFieldModes.Foldout), HideInPlayMode, Required]
    #endif
        [SerializeField]
        internal ContextComponent[] components;
        
        internal ModelsContext models;
        internal ParametersContext parameters;
        internal UnloadPool unload;
        
        private bool _isRemoved;
        
        private void Awake() {
            key = gameObject.name;
            
            fixedTicks = new List<IFixedTick>();
            ticks = new List<ITick>();
            lateTicks = new List<ILateTick>();
            
            ProjectContext.AddContext(this, gameObject.scene.buildIndex);
            
        #if UNITY_EDITOR
            Application.quitting += MarkRemoved;
        #endif
        }
        
        private void FixedUpdate() {
            for (int tickId = 0; tickId < fixedTicks.Count; tickId++) {
                try {
                    fixedTicks[tickId].FixedTick();
                } catch (Exception exception) {
                    Debug.LogError(exception);
                }
            }
        }
        
        private void Update() {
            for (int tickId = 0; tickId < ticks.Count; tickId++) {
                try {
                    ticks[tickId].Tick();
                } catch (Exception exception) {
                    Debug.LogError(exception);
                }
            }
        }
        
        private void LateUpdate() {
            for (int tickId = 0; tickId < lateTicks.Count; tickId++) {
                try {
                    lateTicks[tickId].LateTick();
                } catch (Exception exception) {
                    Debug.LogError(exception);
                }
            }
        }
        
        private void OnDestroy() {
            if (_isRemoved) {
                return;
            }
            
            Remove();
        }
        
        internal void Remove() {
            fixedTicks.Clear();
            ticks.Clear();
            lateTicks.Clear();
            StopAllCoroutines();
            
            _isRemoved = true;
        #if UNITY_EDITOR
            Application.quitting -= MarkRemoved;
        #endif
            ProjectContext.RemoveContext(this, gameObject.scene.buildIndex);
        }
        
        internal abstract void Create();
        
        internal abstract UniTask InitAsync();
        
        internal abstract void Unload();
        
        internal abstract void Connect(View view);
        
        internal abstract void Connect<T1, T2>(T2 system, T1 controller) where T1 : IController where T2 : IController;
        
        internal abstract void Disconnect(View view);
        
        internal abstract void Disconnect<T1, T2>(T2 system, T1 controller) where T1 : IController where T2 : IController;
        
        internal void ConnectLoop(ILoop loop) {
            if (loop is IFixedTick fixedTick) {
                fixedTicks.Add(fixedTick);
            }
            
            if (loop is ITick tick) {
                ticks.Add(tick);
            }
            
            if (loop is ILateTick lateTick) {
                lateTicks.Add(lateTick);
            }
        }
        
        internal void DisconnectLoop(ILoop loop) {
            if (loop is IFixedTick fixedTick) {
                fixedTicks.Remove(fixedTick);
            }
            
            if (loop is ITick tick) {
                ticks.Remove(tick);
            }
            
            if (loop is ILateTick lateTick) {
                lateTicks.Remove(lateTick);
            }
        }
        
    #if UNITY_EDITOR
        
        public virtual void Reset() {
            UnityEditor.EditorUtility.SetDirty(gameObject);
        }
        
        private void MarkRemoved() {
            try {
                fixedTicks.Clear();
                ticks.Clear();
                lateTicks.Clear();
            } catch (Exception) {
                // Do nothing, app closed
            }
            
            try {
                ProjectContext.RemoveContext(this, gameObject.scene.buildIndex);
            } catch (Exception) {
                // Do nothing, app closed
            }
            
            _isRemoved = true;
            Application.quitting -= MarkRemoved;
        }
        
    #endif
        
        public bool Equals(SceneContext other) => other != null && key.Equals(other.key);
        
        public override bool Equals(object obj) => obj is SceneContext other && key.Equals(other.key);
        
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        public override int GetHashCode() => key != null ? key.GetHashCode() : gameObject.GetInstanceID();
    }
}