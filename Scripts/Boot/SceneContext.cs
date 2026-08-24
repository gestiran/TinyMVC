// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TinyMVC.Controllers;
using TinyMVC.Loop;
using TinyMVC.Loop.Extensions;
using TinyMVC.Parameters;
using TinyMVC.Boot.Contexts;
using TinyMVC.Boot.Extensions;
using TinyMVC.Views;
using TinyReactive;
using TinyReactive.Fields;
using TinyUtilities.Logger;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Boot {
    [DefaultExecutionOrder(-50)]
    public abstract class SceneContext : MonoBehaviour, IContext, IUnload, IEquatable<SceneContext> {
        public CancellationToken cancellation => cancellationInternal.Token;
        
        public string key { get; private set; }
        public ControllersContext controllers { get; private set; }
        
        public ViewsContext views { get => viewsInternal; internal set => viewsInternal = value; }
        
        Dictionary<IController, UnloadPool> IContext.unloads => _unloads;
        ControllersContext IContext.controllers { get => controllers; set => controllers = value; }
        
        internal virtual ViewsContext viewsInternal { get; set; }
        internal ModelsContext models { get; set; }
        internal ParametersContext parameters { get; set; }
        
        internal List<IFixedTick> fixedTicks { get; private set; }
        internal List<ITick> ticks { get; private set; }
        internal List<ILateTick> lateTicks { get; private set; }
        
    #if ODIN_INSPECTOR
        [field: ShowInInspector, HideLabel, HideReferenceObjectPicker, HideDuplicateReferenceBox, InlineProperty, HideInEditorMode]
    #endif
        [SerializeField]
        internal ContextComponent[] components;
        
        internal UnloadPool unloadInternal;
        internal CancellationTokenSource cancellationInternal;
        
        private bool _isInitializationComplete;
        private Dictionary<IController, UnloadPool> _unloads;
        private int _sceneId;
        
        private readonly TaskCompletionSource<bool> _initCompletionSource = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        
        private async void Awake() {
            key = gameObject.name;
            
            fixedTicks = new List<IFixedTick>();
            ticks = new List<ITick>();
            lateTicks = new List<ILateTick>();
            _unloads = new Dictionary<IController, UnloadPool>();
            
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
            for (int tickId = 0; tickId < fixedTicks.Count; tickId++) {
                try {
                    fixedTicks[tickId].FixedTick();
                } catch (Exception exception) {
                    DebugUtility.LogError(exception);
                }
            }
        }
        
        private void Update() {
            for (int tickId = 0; tickId < ticks.Count; tickId++) {
                try {
                    ticks[tickId].Tick();
                } catch (Exception exception) {
                    DebugUtility.LogError(exception);
                }
            }
        }
        
        private void LateUpdate() {
            for (int tickId = 0; tickId < lateTicks.Count; tickId++) {
                try {
                    lateTicks[tickId].LateTick();
                } catch (Exception exception) {
                    DebugUtility.LogError(exception);
                }
            }
        }
        
        private void OnDestroy() {
            if (unloadInternal == null) {
            #if UNITY_EDITOR
                if (key == null) {
                    key = $"t:{GetType().Name}";
                }
                
                DebugUtility.LogError($"SceneContext.OnDestroy - Invalid context {key} unload, GameObject disabled!");
            #endif
                return;
            }
            
            if (unloadInternal.isUnloaded) {
                return;
            }
            
            if (this is IGlobalContext) {
                ((IUnload)this).Unload();
                return;
            }
            
            RunRemove();
        }
        
        private IEnumerator InitWindowsProcess() {
            yield return new WaitForEndOfFrame();
            
            try {
                InitWindows();
            } catch (Exception exception) {
                DebugUtility.LogError(new Exception("SceneContext.InitWindows with exception!", exception));
            }
        }
        
        private async void RunRemove() {
            try {
                await this.Remove();
            } catch (Exception exception) {
                DebugUtility.LogError(exception);
            }
        }
        
        public T Add<T>(T unload) where T : IUnload => unloadInternal.Add(unload);
        
        protected virtual void InitWindows() { }
        
        int IContext.sceneId { get => _sceneId; set => _sceneId = value; }
        
        UnloadPool IContext.unloadPool { get => unloadInternal; set => unloadInternal = value; }
        
        CancellationTokenSource IContext.cancellationSource { get => cancellationInternal; set => cancellationInternal = value; }
        
        bool IContext.isInitializationComplete { get => _isInitializationComplete; set => _isInitializationComplete = value; }
        
        Task IContext.initialization => _initCompletionSource.Task;
        
        void IUnload.Unload() {
            StopAllCoroutines();
            
        #if UNITY_EDITOR
            Application.quitting -= MarkRemoved;
        #endif
        }
        
        void IContext.Create() {
            this.Create();
            
            if (this is IGlobalContext) {
                DontDestroyOnLoad(gameObject);
                viewsInternal.ApplyDontDestroyOnLoad();
            }
        }
        
        async Task IContext.InitAsync() {
            try {
                await this.InitAsync();
                
                controllers.CheckAndAdd(fixedTicks);
                controllers.CheckAndAdd(ticks);
                controllers.CheckAndAdd(lateTicks);
                
                viewsInternal.CheckAndAdd(fixedTicks);
                viewsInternal.CheckAndAdd(ticks);
                viewsInternal.CheckAndAdd(lateTicks);
            } catch (Exception exception) {
                DebugUtility.LogException(exception);
            } finally {
                _isInitializationComplete = true;
                _initCompletionSource.TrySetResult(true);
            }
        }
        
        async Task IContext.Remove() {
            fixedTicks.Clear();
            ticks.Clear();
            lateTicks.Clear();
            
            if (this is IGlobalContext) {
                return;
            }
            
            ((IUnload)this).Unload();
            await this.Remove();
        }
        
        void IContext.Connect<T1, T2>(T2 system, T1 controller) => Connect(system, controller);
        
        void IContext.Disconnect<T1, T2>(T2 system, T1 controller) => Disconnect(system, controller);
        
        ControllersContext IContext.CreateControllers() => CreateControllers();
        
        ModelsContext IContext.CreateModels() => CreateModels();
        
        ParametersContext IContext.CreateParameters() => CreateParameters();
        
        ModelsContext IContext.models { get => models; set => models = value; }
        
        ParametersContext IContext.parameters { get => parameters; set => parameters = value; }
        
        IViewsContext IContext.views => viewsInternal;
        
        IContextModule[] IContext.modules => components;
        
        protected abstract ControllersContext CreateControllers();
        
        protected abstract ModelsContext CreateModels();
        
        protected abstract ParametersContext CreateParameters();
        
        internal abstract void Connect(View view);
        
        internal abstract void Disconnect(View view);
        
        internal abstract void Connect<T1, T2>(T2 system, T1 controller) where T1 : IController where T2 : IController;
        
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
        
        public virtual void Reset() => UnityEditor.EditorUtility.SetDirty(gameObject);
        
        private void MarkRemoved() {
            try {
                fixedTicks.Clear();
                ticks.Clear();
                lateTicks.Clear();
            } catch (Exception) {
                // Do nothing, app closed
            }
            
            try {
                if (this is IGlobalContext == false) {
                    ProjectContext.RemoveContext(this, _sceneId);
                }
            } catch (Exception) {
                // Do nothing, app closed
            }
            
            unloadInternal = unloadInternal.Recreate();
            Application.quitting -= MarkRemoved;
        }
        
    #endif
        
        public bool Equals(SceneContext other) => other != null && key == other.key;
        
        public override bool Equals(object obj) => obj is SceneContext other && key == other.key;
        
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        public override int GetHashCode() => key != null ? key.GetHashCode() : gameObject.GetInstanceID();
    }
    
    /// <summary> Typed scene context with a custom views composition. </summary>
    [DisallowMultipleComponent]
    public abstract class SceneContext<TViews> : SceneContext where TViews : ViewsContext {
        [field: SerializeField]
        public new TViews views { get; private set; }
        
        internal override ViewsContext viewsInternal { get => views; set => views = value as TViews; }
        
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
}