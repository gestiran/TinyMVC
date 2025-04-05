using System.Collections.Generic;
using System.Threading.Tasks;
using TinyMVC.Boot.Contexts;
using TinyMVC.Controllers;
using TinyMVC.Dependencies;
using TinyMVC.Loop;
using TinyMVC.Views;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TinyMVC.Boot.Binding;

namespace TinyMVC.Boot {
    [DisallowMultipleComponent]
    public abstract class SceneContext<TViews> : SceneContext where TViews : ViewsContext {
        [field: SerializeField]
        public TViews views { get; private set; }
        
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
            }
        }
        
        internal override async UniTask InitAsync() {
            PreInitResolve();
            
            await views.InitAsync();
            
            await Resolve();
            
            await controllers.BeginPlay();
            await views.BeginPlay();
            
            List<IFixedTick> fixedTicks = new List<IFixedTick>();
            List<ITick> ticks = new List<ITick>();
            List<ILateTick> lateTicks = new List<ILateTick>();
            
            controllers.CheckAndAdd(fixedTicks);
            controllers.CheckAndAdd(ticks);
            controllers.CheckAndAdd(lateTicks);
            
            views.CheckAndAdd(fixedTicks);
            views.CheckAndAdd(ticks);
            views.CheckAndAdd(lateTicks);
            
            ProjectContext.AddFixedTicks(key, fixedTicks);
            ProjectContext.AddTicks(key, ticks);
            ProjectContext.AddLateTicks(key, lateTicks);
        }
        
        internal override void Unload() {
            controllers.Unload();
            views.Unload();
            models.Unload();
        }
        
        protected abstract ControllersContext CreateControllers();
        
        protected abstract ModelsContext CreateModels();
        
        protected abstract ParametersContext CreateParameters();
        
        private void PreInitResolve() {
            List<IResolving> resolvers = new List<IResolving>();
            
            for (int componentId = 0; componentId < components.Length; componentId++) {
                components[componentId].CheckAndAdd(resolvers);
            }
            
            views.CheckAndAdd(resolvers);
            
            ResolveUtility.Resolve(resolvers);
        }
        
        private async UniTask Resolve() {
            if (parameters is IResolving parametersResolving) {
                ResolveUtility.Resolve(parametersResolving);
            }
            
            TryResolveComponents();
            
            List<IDependency> dependenciesParameters = new List<IDependency>(_DEPENDENCIES_CAPACITY);
            List<IDependency> dependenciesViews = new List<IDependency>(_DEPENDENCIES_CAPACITY);
            List<IDependency> dependenciesParametersAndViews = new List<IDependency>(_DEPENDENCIES_CAPACITY);
            
            parameters.Init();
            CreateParametersComponents(parameters.all);
            
            parameters.AddDependencies(dependenciesParameters);
            parameters.AddDependencies(dependenciesParametersAndViews);
            
            ProjectContext.data.Add(key, dependenciesParameters);
            DependencyContainer tempContainer = new DependencyContainer(dependenciesParameters);
            ProjectContext.data.tempContainer = tempContainer;
            ResolveUtility.Resolve(models, tempContainer);
            
            models.CreateBinders();
            CreateBindersComponents(models.binders, models.initContainer);
            
            views.GetDependencies(dependenciesViews);
            views.GetDependencies(dependenciesParametersAndViews);
            
            List<IResolving> resolvers = models.GetBindResolving();
            tempContainer = new DependencyContainer(dependenciesViews);
            ProjectContext.data.tempContainer = tempContainer;
            ResolveUtility.Resolve(resolvers, tempContainer);
            
            List<IDependency> runtimeDependencies = new List<IDependency>(_DEPENDENCIES_CAPACITY);
            
            models.ApplyBindDependencies(key);
            runtimeDependencies.AddRange(models.dependenciesBinded);
            
            tempContainer = new DependencyContainer(runtimeDependencies);
            ProjectContext.data.tempContainer = tempContainer;
            ResolveUtility.Resolve(models, tempContainer);
            ResolveUtility.TryApply(models);
            
            dependenciesParametersAndViews.AddRange(runtimeDependencies);
            models.initContainer = new DependencyContainer(dependenciesParametersAndViews);
            
            models.Create();
            CreateModelsComponents(models.dependencies);
            ProjectContext.data.Add(key, models.dependencies);
            
            models.initContainer = null;
            
            resolvers.Clear();
            
            controllers.Init();
            
            await controllers.InitAsync();
            
            controllers.CheckAndAdd(resolvers);
            views.CheckAndAdd(resolvers);
            
            ResolveUtility.Resolve(resolvers);
            ResolveUtility.TryApply(resolvers);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InstantiateComponents() {
            for (int componentId = 0; componentId < components.Length; componentId++) {
                components[componentId].Instantiate();
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CreateComponentsControllers(List<IController> systems) {
            for (int componentId = 0; componentId < components.Length; componentId++) {
                components[componentId].CreateControllersInternal(systems);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddComponentsViews(List<View> mainViews) {
            for (int componentId = 0; componentId < components.Length; componentId++) {
                components[componentId].AddComponentsViews(mainViews);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void TryResolveComponents() {
            for (int componentId = 0; componentId < components.Length; componentId++) {
                if (components[componentId] is IResolving resolving) {
                    ResolveUtility.Resolve(resolving);
                }
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CreateParametersComponents(List<IDependency> dependencies) {
            for (int componentId = 0; componentId < components.Length; componentId++) {
                components[componentId].CreateParametersInternal(dependencies);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CreateBindersComponents(List<IBinder> binders, DependencyContainer initContainer) {
            for (int componentId = 0; componentId < components.Length; componentId++) {
                components[componentId].CreateBindersInternal(binders, initContainer);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CreateModelsComponents(List<IDependency> dependencies) {
            for (int componentId = 0; componentId < components.Length; componentId++) {
                components[componentId].CreateModelsInternal(dependencies);
            }
        }
        
        internal override void Connect(View view, Action<IResolving> resolve) => views.Connect(view, key, resolve);
        
        internal override void Connect<T1, T2>(T2 system, T1 controller, Action<IResolving> resolve) {
            controllers.Connect(system, controller, key, resolve);
        }
        
        internal override void Disconnect(View view) => views.Disconnect(view, key);
        
        internal override void Disconnect<T1, T2>(T2 system, T1 controller) {
            controllers.Disconnect(system, controller, key);
        }
        
    #if UNITY_EDITOR
        [Button("Generate"), PropertyOrder(20), ShowIn(PrefabKind.InstanceInScene), HideInPlayMode]
        public override void Reset() {
            if (views != null) {
                views.Reset();
            }
            
            base.Reset();
        }
        
    #endif
    }
    
    public abstract class SceneContext : MonoBehaviour, IEquatable<SceneContext> {
        public string key { get; private set; }
        
        [ShowInInspector, HideLabel, HideReferenceObjectPicker, HideDuplicateReferenceBox, InlineProperty, HideInEditorMode]
        internal ControllersContext controllers;
        
        [SerializeField, PropertyOrder(10), InlineEditor(InlineEditorObjectFieldModes.Foldout), HideInPlayMode, Required]
        internal ContextComponent[] components;
        
        internal ModelsContext models;
        internal ParametersContext parameters;
        internal UnloadPool unload;
        
        internal List<IFixedTick> fixedTicks;
        internal List<ITick> ticks;
        internal List<ILateTick> lateTicks;
        
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
            
            if (this is IGlobalContext) {
                DontDestroyOnLoad(this);
            }
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
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Remove() {
            _isRemoved = true;
        #if UNITY_EDITOR
            Application.quitting -= MarkRemoved;
        #endif
            ProjectContext.RemoveContext(this, gameObject.scene.buildIndex);
        }
        
        internal abstract void Create();
        
        internal abstract UniTask InitAsync();
        
        internal abstract void Unload();
        
        internal abstract void Connect(View view, Action<IResolving> resolve);
        
        internal abstract void Connect<T1, T2>(T2 system, T1 controller, Action<IResolving> resolve) where T1 : IController where T2 : IController;
        
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
        
        private void MarkRemoved() => _isRemoved = true;
        
    #endif
        
        public bool Equals(SceneContext other) => other != null && key.Equals(other.key);
        
        public override bool Equals(object obj) => obj is SceneContext other && key.Equals(other.key);
        
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        public override int GetHashCode() => key != null ? key.GetHashCode() : gameObject.GetInstanceID();
    }
}