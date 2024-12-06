using System.Collections.Generic;
using System.Threading.Tasks;
using TinyMVC.Boot.Contexts;
using TinyMVC.Controllers;
using TinyMVC.Dependencies;
using TinyMVC.Loop;
using TinyMVC.Views;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

namespace TinyMVC.Boot {
    [DisallowMultipleComponent]
    public abstract class SceneContext<TViews> : SceneContext where TViews : ViewsContext {
        [field: SerializeField, BoxGroup("Views")]
        public TViews views { get; private set; }
        
        internal override void Create() {
            unload = new UnloadPool();
            
            Add(this);
            
            controllers = CreateControllers();
            models = CreateModels();
            parameters = CreateParameters();
            
            views.Instantiate();
            
            controllers.CreateControllers();
            views.CreateViews();
            
            if (this is IGlobalContext) {
                views.ApplyDontDestroyOnLoad();
            }
        }
        
        internal override async Task InitAsync(int sceneId) {
            PreInitResolve();
            
            await controllers.InitAsync();
            await views.InitAsync();
            
            Resolve();
            
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
            
            ProjectContext.AddFixedTicks(sceneId, fixedTicks);
            ProjectContext.AddTicks(sceneId, ticks);
            ProjectContext.AddLateTicks(sceneId, lateTicks);
        }
        
        internal override void Unload() {
            controllers.Unload();
            views.Unload();
            models.Unload();
            Remove(this);
        }
        
        protected abstract ControllersContext CreateControllers();
        
        protected abstract ModelsContext CreateModels();
        
        protected abstract ParametersContext CreateParameters();
        
        private void PreInitResolve() {
            List<IResolving> resolvers = new List<IResolving>();
            
            controllers.CheckAndAdd(resolvers);
            views.CheckAndAdd(resolvers);
            
            ResolveUtility.Resolve(resolvers);
        }
        
        private void Resolve() {
            if (parameters is IResolving parametersResolving) {
                ResolveUtility.Resolve(parametersResolving);
            }
            
            List<IDependency> dependencies = new List<IDependency>();
            List<IDependency> bindDependencies = new List<IDependency>();
            
            parameters.Init();
            
            parameters.AddDependencies(dependencies);
            parameters.AddDependencies(bindDependencies);
            
            ResolveUtility.Resolve(models, new DependencyContainer(dependencies));
            
            models.CreateBinders();
            
            views.GetDependencies(bindDependencies);
            
            List<IResolving> resolvers = models.GetBindResolving();
            
            ResolveUtility.Resolve(resolvers, new DependencyContainer(bindDependencies));
            ResolveUtility.TryApply(resolvers);
            
            bindDependencies.Clear();
            models.ApplyBindDependencies();
            models.AddDependencies(bindDependencies);
            
            ResolveUtility.Resolve(models, new DependencyContainer(bindDependencies));
            ResolveUtility.TryApply(models);
            
            models.Create();
            
            models.AddDependencies(dependencies);
            
            ProjectContext.data.Add(dependencies);
            resolvers.Clear();
            
            controllers.CheckAndAdd(resolvers);
            views.CheckAndAdd(resolvers);
            
            ResolveUtility.Resolve(resolvers);
            ResolveUtility.TryApply(resolvers);
        }
        
        internal override void Connect(View view, int sceneId, Action<IResolving> resolve) => views.Connect(view, sceneId, resolve);
        
        internal override void Connect<T1, T2>(T2 system, T1 controller, int sceneId, Action<IResolving> resolve) {
            controllers.Connect(system, controller, sceneId, resolve);
        }
        
        internal override void Disconnect(View view, int sceneId) => views.Disconnect(view, sceneId);
        
        internal override void Disconnect<T1, T2>(T2 system, T1 controller, int sceneId) {
            controllers.Disconnect(system, controller, sceneId);
        }
        
    #if UNITY_EDITOR
        [Button("Generate"), ShowIn(PrefabKind.InstanceInScene)]
        public void Reset() {
            if (views != null) {
                views.Reset();
            }
            
            UnityEditor.EditorUtility.SetDirty(gameObject);
        }
        
    #endif
    }
    
    public abstract class SceneContext : MonoBehaviour {
        [ShowInInspector, BoxGroup("Controllers"), HideLabel, HideReferenceObjectPicker, HideDuplicateReferenceBox, InlineProperty, HideInEditorMode]
        internal ControllersContext controllers;
        
        internal ModelsContext models;
        internal ParametersContext parameters;
        internal UnloadPool unload;
        
        private static readonly Dictionary<int, SceneContext> _contexts;
        
        private const int _SCENES_COUNT = 16;
        
        static SceneContext() => _contexts = new Dictionary<int, SceneContext>(_SCENES_COUNT);
        
        internal static SceneContext GetContext(int buildIndex) => _contexts[buildIndex];
        
        internal abstract void Create();
        
        internal abstract Task InitAsync(int sceneId);
        
        internal abstract void Unload();
        
        protected static void Add(SceneContext context) => _contexts.Add(context.gameObject.scene.buildIndex, context);
        
        protected static void Remove(SceneContext context) => _contexts.Remove(context.gameObject.scene.buildIndex);
        
        internal abstract void Connect(View view, int sceneId, Action<IResolving> resolve);
        
        internal abstract void Connect<T1, T2>(T2 system, T1 controller, int sceneId, Action<IResolving> resolve) where T1 : IController where T2 : IController;
        
        internal abstract void Disconnect(View view, int sceneId);
        
        internal abstract void Disconnect<T1, T2>(T2 system, T1 controller, int sceneId) where T1 : IController where T2 : IController;
        
    #if UNITY_EDITOR
        
        [UnityEditor.InitializeOnLoadMethod]
        private static void RevertChanges() => UnityEditor.EditorApplication.playModeStateChanged += PlayModeChange;
        
        private static void PlayModeChange(UnityEditor.PlayModeStateChange state) {
            if (state != UnityEditor.PlayModeStateChange.ExitingPlayMode) {
                return;
            }
            
            _contexts.Clear();
        }
        
    #endif
    }
}