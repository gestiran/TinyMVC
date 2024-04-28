using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TinyMVC.Dependencies;
using TinyMVC.Loop;
using TinyMVC.Loop.Extensions;
using TinyMVC.Views;
using TinyMVC.Views.Generated;
using UnityEngine;

#if ODIN_INSPECTOR && UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

#if UNITY_EDITOR || DEVELOPMENT_BUILD
using TinyMVC.Debugging.Exceptions;
#endif

using UnityObject = UnityEngine.Object;

namespace TinyMVC.Boot.Contexts {
    /// <summary> Contains references to scene objects </summary>
#if ODIN_INSPECTOR && UNITY_EDITOR
    [InlineProperty, HideLabel]
#endif
    [Serializable]
    public abstract class ViewsContext {
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [ListDrawerSettings(HideAddButton = true, ShowFoldout = false), Searchable, Required]
    #endif
        [SerializeField]
        private View[] _assets;
        
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [SceneObjectsOnly, ShowIn(PrefabKind.InstanceInScene), ReadOnly, RequiredIn(PrefabKind.InstanceInScene)]
    #endif
        [SerializeField]
        private View[] _generated;
        
        private List<IView> _mainViews;
        private List<IView> _subViews;
        
        /// <summary> Instantiate new objects to scene before initialization process </summary>
        internal void Instantiate() {
            for (int assetId = 0; assetId < _assets.Length; assetId++) {
                _assets[assetId] = UnityObject.Instantiate(_assets[assetId]);
            }
        }

        internal void GetDependencies(List<IDependency> dependencies) {
            for (int assetId = 0; assetId < _mainViews.Count; assetId++) {
                if (_mainViews[assetId] is IDependency dependency) {
                    dependencies.Add(dependency);
                }
            }
        }
        
        internal void CreateViews() {
            _mainViews = new List<IView>();
            _subViews = new List<IView>();
            
            Create();
            
            _mainViews.AddRange(_assets);
            _mainViews.AddRange(_generated);
        }

        internal async Task InitAsync(int sceneId) {
            for (int viewId = 0; viewId < _mainViews.Count; viewId++) {
                if (_mainViews[viewId] is View view) {
                    view.sceneId = sceneId;
                }
            }
            
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
            try {
            #endif
                
                await _mainViews.TryInitAsync();

            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            } catch (InitException exception) {
                if (exception.other is IView view) {
                    throw new ViewsException(view, exception);
                }

                throw;
            } catch (InitAsyncException exception) {
                if (exception.other is IView view) {
                    throw new ViewsException(view, exception);
                }

                throw;
            }
        #endif
        }

        internal async Task BeginPlay() {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
            try {
            #endif
                
                await _mainViews.TryBeginPlayAsync();

            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            } catch (BeginPlayException exception) {
                if (exception.other is IView view) {
                    throw new ViewsException(view, exception);
                }

                throw;
            } catch (BeginPlayAsyncException exception) {
                if (exception.other is IView view) {
                    throw new ViewsException(view, exception);
                }

                throw;
            }
        #endif
        }

        internal void CheckAndAdd<T>(List<T> list) {
            list.Capacity += list.Count;
            
            for (int viewId = 0; viewId < _mainViews.Count; viewId++) {
                if (_mainViews[viewId] is T view) {
                    list.Add(view);
                }
            }
        }

        internal void ApplyDontDestroyOnLoad() {
            for (int viewId = 0; viewId < _mainViews.Count; viewId++) {
                if (_mainViews[viewId] is IDontDestroyOnLoad && _mainViews[viewId] is MonoBehaviour monoView) {
                    UnityObject.DontDestroyOnLoad(monoView);
                }
            }
        }
        
        internal void Connect(IView view, int sceneId, Action<IResolving> resolve) {
            if (view is IInit init) {
                init.Init();
            }

            if (view is IResolving resolving) {
                resolve(resolving);
            }

            if (view is IBeginPlay beginPlay) {
                beginPlay.BeginPlay();
            }

            if (view is ILoop loop) {
                ProjectContext.current.ConnectLoop(sceneId, loop);
            }
            
            _subViews.Add(view);
        }
        
        internal void Disconnect(IView view, int sceneId) {
            if (view is ILoop loop) {
                ProjectContext.current.DisconnectLoop(sceneId, loop);
            }
            
            if (view is IUnload unload) {
                unload.Unload();
            }
            
            _subViews.Remove(view);
        }
        
        internal void Unload() {
            _subViews.TryUnload();
            _mainViews.TryUnload();
        }

        protected void Add<T>(T view) where T : IView => _mainViews.Add(view);

        /// <summary> Create and connect views to initialization </summary>
        protected abstract void Create();

        protected virtual void Generate() { }
        
    #if UNITY_EDITOR
        
        internal void Generate_Editor() {
            View[] views = UnityObject.FindObjectsOfType<View>(includeInactive: true);
            List<View> generated = new List<View>();
            
            for (int viewId = 0; viewId < views.Length; viewId++) {
                if (views[viewId] is not IGenerated) {
                    continue;
                }

                if (views[viewId] is IGeneratedContext) {
                    generated.Add(views[viewId]); 
                }

                if (views[viewId] is IApplyGenerated target) {
                    target.Reset();
                    UnityEditor.EditorUtility.SetDirty(views[viewId].gameObject);
                } else if (views[viewId] is IApplyGeneratedContext targetContext) {
                    targetContext.Reset();
                    UnityEditor.EditorUtility.SetDirty(views[viewId].gameObject);
                }
            }
            
            _generated = generated.ToArray();
            Generate();
        }
        
    #endif
    }
}