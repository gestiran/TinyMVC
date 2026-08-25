// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using TinyMVC.Controllers;
using TinyMVC.Loop;
using TinyMVC.Loop.Extensions;
using TinyMVC.Parameters;
using TinyMVC.Boot.Contexts;
using TinyMVC.Boot.Extensions;
using TinyMVC.Views;
using TinyReactive;
using TinyReactive.Fields;
using TinyUtilities.Extensions;
using TinyUtilities.Logger;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Boot {
    /// <summary> Typed scene context with a custom views composition. </summary>
    [DisallowMultipleComponent]
    public abstract class SceneContext<TViews> : SceneContext where TViews : ViewsContext {
        [field: SerializeField]
        public new TViews views { get; private set; }
        
        protected override ViewsContext _views => views;
        
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
    public abstract class SceneContext : MonoBehaviour, IContext, IEquatable<SceneContext> {
        public CancellationToken cancellation => _cancellationSource.Token;
        public ViewsContext views => _views;
        
        int IContext.id { get => _sceneId; set => _sceneId = value; }
        
        public string key { get; private set; }
        public ControllersContext controllers { get; private set; }
        
        Dictionary<IController, UnloadPool> IContext.unloads => _unloads;
        UnloadPool IContext.unloadPool => _unload;
        IViewsContext IContext.views => _views;
        IContextModule[] IContext.modules => components;
        ControllersContext IContext.controllers { get => controllers; set => controllers = value; }
        ModelsContext IContext.models { get => _models; set => _models = value; }
        ParametersContext IContext.parameters { get => _parameters; set => _parameters = value; }
        
        protected virtual ViewsContext _views { get; set; }
        private ModelsContext _models { get; set; }
        private ParametersContext _parameters { get; set; }
        private List<IFixedTick> _fixedTicks { get; set; }
        private List<ITick> _ticks { get; set; }
        private List<ILateTick> _lateTicks { get; set; }
        
    #if ODIN_INSPECTOR
        [field: ShowInInspector, HideLabel, HideReferenceObjectPicker, HideDuplicateReferenceBox, InlineProperty, HideInEditorMode]
    #endif
        [SerializeField]
        internal ContextComponent[] components;
        
        private int _sceneId;
        private UnloadPool _unload;
        private Dictionary<IController, UnloadPool> _unloads;
        private CancellationTokenSource _cancellationSource;
        private TaskCompletionSource<bool> _initializationStatus;
        
        private const int _INITIALIZATION_TIMEOUT = 4800;
        
        private async void Awake() {
            key = gameObject.name;
            
            _fixedTicks = new List<IFixedTick>();
            _ticks = new List<ITick>();
            _lateTicks = new List<ILateTick>();
            _unloads = new Dictionary<IController, UnloadPool>();
            _unload = new UnloadPool();
            _cancellationSource = new CancellationTokenSource();
            _initializationStatus = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            
        #if UNITY_EDITOR
            if (TinyMVCParameters.LoadFromResources().isEnableAutoReload) {
                Reset();
            }
        #endif
            
            await ProjectContext.AddContext(this, gameObject.scene.buildIndex);
            
        #if UNITY_EDITOR
            Application.quitting += MarkRemoved;
        #endif
        }
        
        private void Start() => StartCoroutine(InitWindowsProcess());
        
        private void FixedUpdate() {
            for (int tickId = 0; tickId < _fixedTicks.Count; tickId++) {
                try {
                    _fixedTicks[tickId].FixedTick();
                } catch (Exception exception) {
                    DebugUtility.LogError(exception);
                }
            }
        }
        
        private void Update() {
            for (int tickId = 0; tickId < _ticks.Count; tickId++) {
                try {
                    _ticks[tickId].Tick();
                } catch (Exception exception) {
                    DebugUtility.LogError(exception);
                }
            }
        }
        
        private void LateUpdate() {
            for (int tickId = 0; tickId < _lateTicks.Count; tickId++) {
                try {
                    _lateTicks[tickId].LateTick();
                } catch (Exception exception) {
                    DebugUtility.LogError(exception);
                }
            }
        }
        
        private void OnDestroy() {
            if (_unload == null) {
            #if UNITY_EDITOR
                if (key == null) {
                    key = $"t:{GetType().Name}";
                }
                
                DebugUtility.LogError($"SceneContext.OnDestroy - Invalid context {key} unload, GameObject disabled!");
            #endif
                return;
            }
            
            if (_unload.isUnloaded) {
                return;
            }
            
            try {
                RemoveProcess(_cancellationSource.Token).Forget();
            } catch (Exception exception) {
                DebugUtility.LogError(exception);
            }
        }
        
        private IEnumerator InitWindowsProcess() {
            yield return new WaitForEndOfFrame();
            
            try {
                InitWindows();
            } catch (Exception exception) {
                DebugUtility.LogError(new Exception("SceneContext.InitWindows with exception!", exception));
            }
        }
        
        public T Add<T>(T unload) where T : IUnload => _unload.Add(unload);
        
        protected virtual void InitWindows() { }
        
        void IContext.Create() {
            this.Create();
            
            if (this is IGlobalContext) {
                DontDestroyOnLoad(gameObject);
                _views.ApplyDontDestroyOnLoad();
            }
        }
        
        async Task IContext.InitAsync() {
            try {
                await this.InitAsync();
                
                controllers.CheckAndAdd(_fixedTicks);
                controllers.CheckAndAdd(_ticks);
                controllers.CheckAndAdd(_lateTicks);
                
                _views.CheckAndAdd(_fixedTicks);
                _views.CheckAndAdd(_ticks);
                _views.CheckAndAdd(_lateTicks);
            } catch (Exception exception) {
                DebugUtility.LogException(exception);
            } finally {
                _initializationStatus.TrySetResult(true);
            }
        }
        
        async Task IContext.Remove() => await RemoveProcess(_cancellationSource.Token);
        
        private async UniTask RemoveProcess(CancellationToken cancellationToken) {
            int ticks = 0;
            
            while (_initializationStatus.Task.IsCompleted == false) {
                await UniTask.Yield(cancellationToken);
                
                if (ticks++ < _INITIALIZATION_TIMEOUT) {
                    continue;
                }
                
                DebugUtility.LogException(new TimeoutException($"Context '{key}' did not finish initialization!"));
                break;
            }
            
            StopAllCoroutines();
            
            try {
                _fixedTicks.Clear();
                _ticks.Clear();
                _lateTicks.Clear();
            } catch (Exception exception) {
                DebugUtility.LogWarning(exception);
            }
            
            if (this is IGlobalContext) {
                return;
            }
            
            _cancellationSource = _cancellationSource.Reset();
            
        #if UNITY_EDITOR
            Application.quitting -= MarkRemoved;
        #endif
            
            ProjectContext.RemoveContext(this, _sceneId);
        }
        
        void IContext.Connect<T1, T2>(T2 system, T1 controller) => Connect(system, controller);
        
        void IContext.Disconnect<T1, T2>(T2 system, T1 controller) => Disconnect(system, controller);
        
        ControllersContext IContext.CreateControllers() => CreateControllers();
        
        ModelsContext IContext.CreateModels() => CreateModels();
        
        ParametersContext IContext.CreateParameters() => CreateParameters();
        
        protected abstract ControllersContext CreateControllers();
        
        protected abstract ModelsContext CreateModels();
        
        protected abstract ParametersContext CreateParameters();
        
        internal abstract void Connect(View view);
        
        internal abstract void Disconnect(View view);
        
        internal abstract void Connect<T1, T2>(T2 system, T1 controller) where T1 : IController where T2 : IController;
        
        internal abstract void Disconnect<T1, T2>(T2 system, T1 controller) where T1 : IController where T2 : IController;
        
        internal void ConnectLoop(ILoop loop) {
            if (loop is IFixedTick fixedTick) {
                _fixedTicks.Add(fixedTick);
            }
            
            if (loop is ITick tick) {
                _ticks.Add(tick);
            }
            
            if (loop is ILateTick lateTick) {
                _lateTicks.Add(lateTick);
            }
        }
        
        internal void DisconnectLoop(ILoop loop) {
            if (loop is IFixedTick fixedTick) {
                _fixedTicks.Remove(fixedTick);
            }
            
            if (loop is ITick tick) {
                _ticks.Remove(tick);
            }
            
            if (loop is ILateTick lateTick) {
                _lateTicks.Remove(lateTick);
            }
        }
        
    #if UNITY_EDITOR
        
        public virtual void Reset() => UnityEditor.EditorUtility.SetDirty(gameObject);
        
        private void MarkRemoved() {
            try {
                _fixedTicks.Clear();
                _ticks.Clear();
                _lateTicks.Clear();
            } catch (Exception) {
                // Do nothing, app closed
            }
            
            try {
                _cancellationSource = _cancellationSource.Reset();
                ProjectContext.RemoveContext(this, _sceneId);
            } catch (Exception) {
                // Do nothing, app closed
            }
            
            Application.quitting -= MarkRemoved;
        }
        
    #endif
        
        public bool Equals(SceneContext other) => other != null && key == other.key;
        
        public override bool Equals(object obj) => obj is SceneContext other && key == other.key;
        
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        public override int GetHashCode() => key != null ? key.GetHashCode() : gameObject.GetInstanceID();
    }
}