using System;
using System.Collections.Generic;
using TinyMVC.Dependencies;
using TinyMVC.Loop;
using TinyMVC.ReactiveFields;
using UnityEngine;
using UnityEngine.SceneManagement;

#if ODIN_INSPECTOR && UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Boot {
    [DisallowMultipleComponent]
    internal sealed class ProjectContext : MonoBehaviour {
        private List<BootstrapContext> _contexts;
        
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [BoxGroup("Debug"), ShowInInspector, HideInEditorMode, HideReferenceObjectPicker, HideDuplicateReferenceBox]
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, DraggableItems = false, ShowFoldout = false)]
    #endif
        private List<DependencyContext> _dependencies;

        private List<FixedTickContext> _fixedTicks;
        private List<TickContext> _ticks;
        private List<LateTickContext> _lateTicks;

        private sealed class BootstrapContext : ContextLink<IContext> {
            public BootstrapContext(Scene scene, IContext context) : base(scene, context) { }
        }

        private sealed class DependencyContext : ContextLink<DependencyContainer> {
            public DependencyContext(Scene scene, DependencyContainer context) : base(scene, context) { }
        }

        private sealed class FixedTickContext : ContextLink<List<IFixedTick>> {
            public FixedTickContext(Scene scene, List<IFixedTick> context) : base(scene, context) { }
        }

        private sealed class TickContext : ContextLink<List<ITick>> {
            public TickContext(Scene scene, List<ITick> context) : base(scene, context) { }
        }
        
        private sealed class LateTickContext : ContextLink<List<ILateTick>> {
            public LateTickContext(Scene scene, List<ILateTick> context) : base(scene, context) { }
        }

        private abstract class ContextLink<T> : IEquatable<ContextLink<T>>, IEquatable<Scene> {
        #if ODIN_INSPECTOR && UNITY_EDITOR
            [HideInEditorMode, HideInPlayMode]
        #endif
            private readonly int _sceneId;

        #if ODIN_INSPECTOR && UNITY_EDITOR
            private string _label;
            
            [ShowInInspector, Title("@" + nameof(_label)), InlineProperty, HideLabel, HideInEditorMode, HideReferenceObjectPicker, HideDuplicateReferenceBox]
        #endif
            public readonly T context;

            protected ContextLink(Scene scene, T context) {
                _sceneId = scene.buildIndex;
                this.context = context;
            #if ODIN_INSPECTOR && UNITY_EDITOR
                _label = scene.name;
            #endif
            }

            public bool Equals(ContextLink<T> other) => other != null && _sceneId.Equals(other._sceneId);

            public bool Equals(Scene other) => _sceneId.Equals(other.buildIndex);

            public override bool Equals(object obj) => obj is ContextLink<T> other && _sceneId.Equals(other._sceneId);

            public override int GetHashCode() => _sceneId;
        }

        [RuntimeInitializeOnLoadMethod]
        internal static void CreateContext() {
            GameObject context = new GameObject("ProjectContext");
            context.AddComponent<ProjectContext>();
        }
        
        private void Awake() {
            DontDestroyOnLoad(this);
            
            _contexts = new List<BootstrapContext>();
            _dependencies = new List<DependencyContext>();
            _fixedTicks = new List<FixedTickContext>();
            _ticks = new List<TickContext>();
            _lateTicks = new List<LateTickContext>();
        }

        private void Start() {
            InitScene(SceneManager.GetActiveScene(), LoadSceneMode.Single);
            
            SceneManager.sceneLoaded += InitScene;
            SceneManager.sceneUnloaded += UnloadScene;
        }

        private void FixedUpdate() {
            foreach (FixedTickContext context in _fixedTicks) {
                foreach (IFixedTick fixedTick in context.context) {
                    fixedTick.FixedTick();
                }
            }
        }

        private void Update() {
        #if UNITY_EDITOR
            ObservedTestUtility.Next();
        #endif
            
            for (int contextId = 0; contextId < _ticks.Count; contextId++) {
                for (int tickId = 0; tickId < _ticks[contextId].context.Count; tickId++) {
                    _ticks[contextId].context[tickId].Tick();
                }
            }
        }

        private void LateUpdate() {
            for (int contextId = 0; contextId < _lateTicks.Count; contextId++) {
                for (int tickId = 0; tickId < _lateTicks[contextId].context.Count; tickId++) {
                    _lateTicks[contextId].context[tickId].LateTick();
                }
            }
        }

        internal void AddContainer(ref Scene scene, DependencyContainer container) {
            if (TryGetContext(_dependencies, ref scene, out DependencyContainer _, out int _)) {
                return;
            }
            
            _dependencies.Add(new DependencyContext(scene, container));
        }

        internal void AddFixedTicks(ref Scene scene, List<IFixedTick> ticks) {
            if (TryGetContext(_fixedTicks, ref scene, out List<IFixedTick> _, out int _)) {
                return;
            }

            if (ticks.Count <= 0) {
                return;
            }

            _fixedTicks.Add(new FixedTickContext(scene, ticks));
        }

        internal void AddTicks(ref Scene scene, List<ITick> ticks) {
            if (TryGetContext(_ticks, ref scene, out List<ITick> _, out int _)) {
                return;
            }
            
            if (ticks.Count <= 0) {
                return;
            }

            _ticks.Add(new TickContext(scene, ticks));
        }
        
        internal void AddLateTicks(ref Scene scene, List<ILateTick> ticks) {
            if (TryGetContext(_lateTicks, ref scene, out List<ILateTick> _, out int _)) {
                return;
            }
            
            if (ticks.Count <= 0) {
                return;
            }

            _lateTicks.Add(new LateTickContext(scene, ticks));
        }

        internal void Resolve(IResolving resolving) => ResolveUtility.Resolve(resolving, CreateContainers());

        internal void Resolve(List<IResolving> resolving) => ResolveUtility.Resolve(resolving, CreateContainers());
        
        internal void ResolveWithoutApply(IResolving resolving, DependencyContainer container) => ResolveUtility.ResolveWithoutApply(resolving, container, CreateContainers());

        private DependencyContainer[] CreateContainers() {
            DependencyContainer[] containers = new DependencyContainer[_dependencies.Count];

            for (int containerId = 0; containerId < _dependencies.Count; containerId++) {
                containers[containerId] = _dependencies[containerId].context;
            }

            return containers;
        }

        private void InitScene(Scene scene, LoadSceneMode mode) {
            if (TryGetContext(_contexts, ref scene, out IContext _, out int _)) {
                return;
            }
            
            if (!TryFindSceneContext(ref scene, out IContext context)) {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.LogError($"ProjectContext.InitScene() - {scene.name} not contain Bootstrap!");
            #endif
                return;
            }
            
            context.Create();
            context.Init(this, ref scene);

            _contexts.Add(new BootstrapContext(scene, context));
        }

        private bool TryFindSceneContext(ref Scene scene, out IContext context) {
            GameObject[] rootObjects = scene.GetRootGameObjects();

            for (int rootId = 0; rootId < rootObjects.Length; rootId++) {
                context = rootObjects[rootId].GetComponent<IContext>();

                if (context == null) {
                    continue;
                }

                return true;
            }
            
            context = null;
            return false;
        }

        private void UnloadScene(Scene scene) {
            if (!TryGetContext(_contexts, ref scene, out IContext context, out int contextId)) {
                return;
            }

            if (context is IGlobalContext) {
                return;
            }
            
            context.Unload();

            if (TryGetContext(_dependencies, ref scene, out DependencyContainer _, out int modelsId)) {
                _dependencies.RemoveAt(modelsId);
            }

            if (TryGetContext(_fixedTicks, ref scene, out List<IFixedTick> _, out int fixedTickId)) {
                _fixedTicks.RemoveAt(fixedTickId);
            }

            if (TryGetContext(_ticks, ref scene, out List<ITick> _, out int tickId)) {
                _ticks.RemoveAt(tickId);
            }
            
            if (TryGetContext(_lateTicks, ref scene, out List<ILateTick> _, out int lateTickId)) {
                _lateTicks.RemoveAt(lateTickId);
            }

            _contexts.RemoveAt(contextId);
        }

        private bool TryGetContext<T1, T2>(List<T1> list, ref Scene target, out T2 context, out int id) where T1 : ContextLink<T2> {
            for (int i = 0; i < list.Count; i++) {
                if (!list[i].Equals(target)) {
                    continue;
                }

                context = list[i].context;
                id = i;
                return true;
            }

            context = default;
            id = default;
            return false;
        }
    }
}