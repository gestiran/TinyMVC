using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TinyMVC.Boot.Binding;
using TinyMVC.Dependencies;
using TinyMVC.Types;
using TinyMVC.Views;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityObject = UnityEngine.Object;

namespace TinyMVC.Boot {
    public static class ProjectCreate {
        public static bool New<T>(ViewLink link, out T model) where T : IDependency, new() {
            View[] views = UnityObject.Instantiate(link.link).GetComponents<View>();
            int sceneId = SceneManager.GetActiveScene().buildIndex;
            return TryExtractModel(views, sceneId, FillDependencies(views, Array.Empty<IDependency>()), out model);
        }
        
        public static bool New<T>(ViewLink link, out T model, params IDependency[] dependencies) where T : IDependency, new() {
            View[] views = UnityObject.Instantiate(link.link).GetComponents<View>();
            int sceneId = SceneManager.GetActiveScene().buildIndex;
            return TryExtractModel(views, sceneId, FillDependencies(views, dependencies), out model);
        }
        
        public static bool New<T1, T2>(ViewLink link, out (T1, T2) model) where T1 : IDependency, new() where T2 : IDependency, new() {
            View[] views = UnityObject.Instantiate(link.link).GetComponents<View>();
            int sceneId = SceneManager.GetActiveScene().buildIndex;
            List<IDependency> result = FillDependencies(views, Array.Empty<IDependency>());
            
            if (TryExtractModel(views, sceneId, result, out T1 first) && TryExtractModel(views, sceneId, result, out T2 second)) {
                model = (first, second);
                return true;
            }
            
            model = default;
            return false;
        }
        
        public static bool New<T1, T2>(ViewLink link, out (T1, T2) model, params IDependency[] dependencies) where T1 : IDependency, new() where T2 : IDependency, new() {
            View[] views = UnityObject.Instantiate(link.link).GetComponents<View>();
            int sceneId = SceneManager.GetActiveScene().buildIndex;
            List<IDependency> result = FillDependencies(views, dependencies);
            
            if (TryExtractModel(views, sceneId, result, out T1 first) && TryExtractModel(views, sceneId, result, out T2 second)) {
                model = (first, second);
                return true;
            }
            
            model = default;
            return false;
        }
        
        public static bool New<T>(ViewLink link, ViewLink parent, out T model) where T : IDependency, new() {
            View[] views = UnityObject.Instantiate(link.link, parent.link.transform).GetComponents<View>();
            int sceneId = SceneManager.GetActiveScene().buildIndex;
            return TryExtractModel(views, sceneId, FillDependencies(views, Array.Empty<IDependency>()), out model);
        }
        
        public static bool New<T>(ViewLink link, ViewLink parent, out T model, params IDependency[] dependencies) where T : IDependency, new() {
            View[] views = UnityObject.Instantiate(link.link, parent.link.transform).GetComponents<View>();
            int sceneId = SceneManager.GetActiveScene().buildIndex;
            return TryExtractModel(views, sceneId, FillDependencies(views, dependencies), out model);
        }
        
        public static bool New<T1, T2>(ViewLink link, ViewLink parent, out (T1, T2) model) where T1 : IDependency, new() where T2 : IDependency, new() {
            View[] views = UnityObject.Instantiate(link.link, parent.link.transform).GetComponents<View>();
            int sceneId = SceneManager.GetActiveScene().buildIndex;
            List<IDependency> result = FillDependencies(views, Array.Empty<IDependency>());
            
            if (TryExtractModel(views, sceneId, result, out T1 first) && TryExtractModel(views, sceneId, result, out T2 second)) {
                model = (first, second);
                return true;
            }
            
            model = default;
            return false;
        }
        
        public static bool New<T1, T2>(ViewLink link, ViewLink parent, out (T1, T2) model, params IDependency[] dependencies)
            where T1 : IDependency, new() where T2 : IDependency, new() {
            View[] views = UnityObject.Instantiate(link.link, parent.link.transform).GetComponents<View>();
            int sceneId = SceneManager.GetActiveScene().buildIndex;
            List<IDependency> result = FillDependencies(views, dependencies);
            
            if (TryExtractModel(views, sceneId, result, out T1 first) && TryExtractModel(views, sceneId, result, out T2 second)) {
                model = (first, second);
                return true;
            }
            
            model = default;
            return false;
        }
        
        public static bool New<T>(ViewLink link, Vector3 position, Quaternion rotation, out T model) where T : IDependency, new() {
            View[] views = UnityObject.Instantiate(link.link, position, rotation).GetComponents<View>();
            int sceneId = SceneManager.GetActiveScene().buildIndex;
            return TryExtractModel(views, sceneId, FillDependencies(views, Array.Empty<IDependency>()), out model);
        }
        
        public static bool New<T>(ViewLink link, Vector3 position, Quaternion rotation, out T model, params IDependency[] dependencies) where T : IDependency, new() {
            View[] views = UnityObject.Instantiate(link.link, position, rotation).GetComponents<View>();
            int sceneId = SceneManager.GetActiveScene().buildIndex;
            return TryExtractModel(views, sceneId, FillDependencies(views, dependencies), out model);
        }
        
        public static bool New<T1, T2>(ViewLink link, Vector3 position, Quaternion rotation, out (T1, T2) model)
            where T1 : IDependency, new() where T2 : IDependency, new() {
            View[] views = UnityObject.Instantiate(link.link, position, rotation).GetComponents<View>();
            int sceneId = SceneManager.GetActiveScene().buildIndex;
            
            List<IDependency> result = FillDependencies(views, Array.Empty<IDependency>());
            
            if (TryExtractModel(views, sceneId, result, out T1 first) && TryExtractModel(views, sceneId, result, out T2 second)) {
                model = (first, second);
                return true;
            }
            
            model = default;
            return false;
        }
        
        public static bool New<T1, T2>(ViewLink link, Vector3 position, Quaternion rotation, out (T1, T2) model, params IDependency[] dependencies)
            where T1 : IDependency, new() where T2 : IDependency, new() {
            View[] views = UnityObject.Instantiate(link.link, position, rotation).GetComponents<View>();
            int sceneId = SceneManager.GetActiveScene().buildIndex;
            
            List<IDependency> result = FillDependencies(views, dependencies);
            
            if (TryExtractModel(views, sceneId, result, out T1 first) && TryExtractModel(views, sceneId, result, out T2 second)) {
                model = (first, second);
                return true;
            }
            
            model = default;
            return false;
        }
        
        public static bool New<T>(ViewLink link, Vector3 position, Quaternion rotation, ViewLink parent, out T model) where T : IDependency, new() {
            View[] views = UnityObject.Instantiate(link.link, position, rotation, parent.link.transform).GetComponents<View>();
            int sceneId = SceneManager.GetActiveScene().buildIndex;
            return TryExtractModel(views, sceneId, FillDependencies(views, Array.Empty<IDependency>()), out model);
        }
        
        public static bool New<T>(ViewLink link, Vector3 position, Quaternion rotation, ViewLink parent, out T model, params IDependency[] dependencies) where T : IDependency, new() {
            View[] views = UnityObject.Instantiate(link.link, position, rotation, parent.link.transform).GetComponents<View>();
            int sceneId = SceneManager.GetActiveScene().buildIndex;
            return TryExtractModel(views, sceneId, FillDependencies(views, dependencies), out model);
        }
        
        public static bool New<T1, T2>(ViewLink link, Vector3 position, Quaternion rotation, ViewLink parent, out (T1, T2) model)
            where T1 : IDependency, new() where T2 : IDependency, new() {
            View[] views = UnityObject.Instantiate(link.link, position, rotation, parent.link.transform).GetComponents<View>();
            int sceneId = SceneManager.GetActiveScene().buildIndex;
            
            List<IDependency> result = FillDependencies(views, Array.Empty<IDependency>());
            
            if (TryExtractModel(views, sceneId, result, out T1 first) && TryExtractModel(views, sceneId, result, out T2 second)) {
                model = (first, second);
                return true;
            }
            
            model = default;
            return false;
        }
        
        public static bool New<T1, T2>(ViewLink link, Vector3 position, Quaternion rotation, ViewLink parent, out (T1, T2) model, params IDependency[] dependencies)
            where T1 : IDependency, new() where T2 : IDependency, new() {
            View[] views = UnityObject.Instantiate(link.link, position, rotation, parent.link.transform).GetComponents<View>();
            int sceneId = SceneManager.GetActiveScene().buildIndex;
            
            List<IDependency> result = FillDependencies(views, dependencies);
            
            if (TryExtractModel(views, sceneId, result, out T1 first) && TryExtractModel(views, sceneId, result, out T2 second)) {
                model = (first, second);
                return true;
            }
            
            model = default;
            return false;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryExtractModel<T>(View[] views, int sceneId, List<IDependency> dependencies, out T model) where T : IDependency, new() {
            if (dependencies.Count > 0) {
                IDependency[] array = dependencies.ToArray();
                
                for (int viewId = 0; viewId < views.Length; viewId++) {
                    if (ProjectBinding.TryBind(out T extracted, array)) {
                        SceneContext.GetContext(sceneId).Connect(views[viewId], sceneId, resolving => ResolveUtility.Resolve(resolving, new DependencyContainer(extracted)));
                        views[viewId].connectState = View.ConnectState.Connected;
                        model = extracted;
                        return true;
                    }
                }
            } else {
                for (int viewId = 0; viewId < views.Length; viewId++) {
                    if (ProjectBinding.TryBind(out T extracted)) {
                        SceneContext.GetContext(sceneId).Connect(views[viewId], sceneId, resolving => ResolveUtility.Resolve(resolving, new DependencyContainer(extracted)));
                        views[viewId].connectState = View.ConnectState.Connected;
                        model = extracted;
                        return true;
                    }
                }
            }
            
            model = default;
            return false;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static List<IDependency> FillDependencies(View[] views, IDependency[] dependencies) {
            List<IDependency> result = new List<IDependency>(dependencies);
            
            for (int viewId = 0; viewId < views.Length; viewId++) {
                if (views[viewId] is IDependency dependency) {
                    result.Add(dependency);
                }
            }
            
            return result;
        }
    }
}