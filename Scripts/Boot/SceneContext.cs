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
using TinyMVC.Dependencies;
using TinyMVC.Views;
using TinyReactive;
using TinyUtilities.Extensions;
using TinyUtilities.Logger;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Boot {
    /// <summary> Scene context with views composition. </summary>
    [DisallowMultipleComponent, DefaultExecutionOrder(-50)]
    public abstract class SceneContext : MonoBehaviour, IContext, IEquatable<SceneContext> {
        public CancellationToken cancellation => _cancellationSource.Token;
        
        [field: SerializeField]
        public ViewsContext views { get; private set; }
        
        public string key { get; private set; }
        
        IViewsContext IContext.views => views;
        ControllersContext IContext.controllers => _controllers;
        ModelsContext IContext.models => _models;
        ParametersContext IContext.parameters => _parameters;
        UnloadPool IContext.unloadPool => _unload;
        Dictionary<IController, UnloadPool> IContext.unloads => _unloads;
        
    #if ODIN_INSPECTOR
        [PropertyOrder(10), InlineEditor(InlineEditorObjectFieldModes.Foldout), HideInPlayMode, Required]
    #endif
        [SerializeField]
        internal ContextComponent[] components;
        
    #if ODIN_INSPECTOR
        [ShowInInspector, HideLabel, HideReferenceObjectPicker, HideDuplicateReferenceBox, InlineProperty, HideInEditorMode]
    #endif
        private ControllersContext _controllers;
        
        private int _sceneId;
        
        private List<IFixedTick> _fixedTicks;
        private List<ITick> _ticks;
        private List<ILateTick> _lateTicks;
        
        private ModelsContext _models;
        private ParametersContext _parameters;
        
        private UnloadPool _unload;
        private Dictionary<IController, UnloadPool> _unloads;
        private CancellationTokenSource _cancellationSource;
        private TaskCompletionSource<bool> _initializationStatus;
        
        private const int _INITIALIZATION_TIMEOUT = 4800;
        
        private async void Awake() {
            key = gameObject.name;
            _sceneId = gameObject.scene.buildIndex;
            
            _fixedTicks = new List<IFixedTick>();
            _ticks = new List<ITick>();
            _lateTicks = new List<ILateTick>();
            _unload = new UnloadPool();
            _unloads = new Dictionary<IController, UnloadPool>();
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
        
        void IContext.Create() {
            _controllers = CreateControllers();
            _models = CreateModels();
            _parameters = CreateParameters();
            
            this.Create();
            
            if (this is IGlobalContext) {
                DontDestroyOnLoad(gameObject);
                views.ApplyDontDestroyOnLoad();
            }
        }
        
        async Task IContext.InitAsync() {
            try {
                await this.InitAsync();
                
                _controllers.CheckAndAdd(_fixedTicks);
                _controllers.CheckAndAdd(_ticks);
                _controllers.CheckAndAdd(_lateTicks);
                
                views.CheckAndAdd(_fixedTicks);
                views.CheckAndAdd(_ticks);
                views.CheckAndAdd(_lateTicks);
            } catch (Exception exception) {
                DebugUtility.LogException(exception);
            } finally {
                _initializationStatus.TrySetResult(true);
            }
        }
        
        IEnumerable<IContextComponent> IContext.Components() {
            for (int componentId = 0; componentId < components.Length; componentId++) {
                yield return components[componentId];
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
        
        public T Add<T>(T unload) where T : IUnload => _unload.Add(unload);
        
        private IEnumerator InitWindowsProcess() {
            yield return new WaitForEndOfFrame();
            
            try {
                InitWindows();
            } catch (Exception exception) {
                DebugUtility.LogError(new Exception("SceneContext.InitWindows with exception!", exception));
            }
        }
        
        protected virtual void InitWindows() { }
        
        protected abstract ControllersContext CreateControllers();
        
        protected abstract ModelsContext CreateModels();
        
        protected abstract ParametersContext CreateParameters();
        
        internal void Connect(View view) => views.Connect(view, ConnectLoop);
        
        internal void Disconnect(View view) => views.Disconnect(view, DisconnectLoop);
        
        void IContext.Connect<T1, T2>(T2 system, T1 controller) {
            if (controller is IInit init) {
                init.Init();
            }
            
            if (controller is IApplyResolving applyResolving) {
                applyResolving.ApplyResolving();
            }
            
            if (controller is IBeginPlay beginPlay) {
                beginPlay.BeginPlay();
            }
            
            if (controller is ILoop loop) {
                ConnectLoop(loop);
            }
            
            string systemName = system.GetType().Name;
            
            if (_controllers.controllers.TryGetValue(systemName, out List<IController> all)) {
                all.Add(controller);
            } else {
                _controllers.controllers.Add(systemName, new List<IController>() { controller });
            }
        }
        
        void IContext.Disconnect<T1, T2>(T2 system, T1 controller) {
            if (controller is ILoop loop) {
                DisconnectLoop(loop);
            }
            
            if (controller is IUnload unload) {
                unload.Unload();
            }
            
            if (ProjectContext.scene.unloads.Remove(controller, out UnloadPool globalUnload)) {
                globalUnload.Unload();
            }
            
            if (_controllers.controllers.TryGetValue(system.GetType().Name, out List<IController> all)) {
                if (_controllers.controllers.TryGetValue(controller.GetType().Name, out List<IController> subControllers)) {
                    for (int controllerId = subControllers.Count - 1; controllerId >= 0; controllerId--) {
                        DisconnectNR(system, subControllers[controllerId]);
                    }
                }
                
                all.Remove(controller);
            }
        }
        
        private void ConnectLoop(ILoop loop) {
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
        
        private void DisconnectLoop(ILoop loop) {
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
        
        private void DisconnectNR<T1, T2>(T2 system, T1 controller) where T1 : IController where T2 : IController {
            (this as IContext).Disconnect(system, controller);
        }
        
    #if UNITY_EDITOR
        
    #if ODIN_INSPECTOR
        [Button("Generate"), PropertyOrder(20), ShowIn(PrefabKind.InstanceInScene), HideInPlayMode]
    #endif
        public void Reset() {
            if (views != null) {
                views.Reset();
                Generate();
                UnityEditor.EditorUtility.SetDirty(gameObject);
            }
        }
        
        protected virtual void Generate() { }
        
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