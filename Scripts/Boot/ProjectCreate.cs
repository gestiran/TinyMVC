using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TinyMVC.Boot.Binding;
using TinyMVC.Dependencies;
using TinyMVC.Types;
using TinyMVC.Views;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace TinyMVC.Boot {
    public static class ProjectCreate {
        public static bool New<T>(ViewLink link, out T model) where T : IDependency, new() {
            View[] views = UnityObject.Instantiate(link.link).GetComponents<View>();
            string contextKey = ProjectContext.activeContext.key;
            return TryExtractModel(views, contextKey, FillDependencies(views, Array.Empty<IDependency>()), out model);
        }
        
        public static bool New<T>(ViewLink link, out T model, params IDependency[] dependencies) where T : IDependency, new() {
            View[] views = UnityObject.Instantiate(link.link).GetComponents<View>();
            string contextKey = ProjectContext.activeContext.key;
            return TryExtractModel(views, contextKey, FillDependencies(views, dependencies), out model);
        }
        
        public static bool New<T1, T2>(ViewLink link, out (T1, T2) model) where T1 : IDependency, new() where T2 : IDependency, new() {
            View[] views = UnityObject.Instantiate(link.link).GetComponents<View>();
            string contextKey = ProjectContext.activeContext.key;
            List<IDependency> result = FillDependencies(views, Array.Empty<IDependency>());
            
            if (TryExtractModel(views, contextKey, result, out T1 first) && TryExtractModel(views, contextKey, result, out T2 second)) {
                model = (first, second);
                return true;
            }
            
            model = default;
            return false;
        }
        
        public static bool New<T1, T2>(ViewLink link, out (T1, T2) model, params IDependency[] dependencies) where T1 : IDependency, new() where T2 : IDependency, new() {
            View[] views = UnityObject.Instantiate(link.link).GetComponents<View>();
            string contextKey = ProjectContext.activeContext.key;
            List<IDependency> result = FillDependencies(views, dependencies);
            
            if (TryExtractModel(views, contextKey, result, out T1 first) && TryExtractModel(views, contextKey, result, out T2 second)) {
                model = (first, second);
                return true;
            }
            
            model = default;
            return false;
        }
        
        public static bool New<T>(ViewLink link, ViewLink parent, out T model) where T : IDependency, new() {
            View[] views = UnityObject.Instantiate(link.link, parent.link.transform).GetComponents<View>();
            string contextKey = ProjectContext.activeContext.key;
            return TryExtractModel(views, contextKey, FillDependencies(views, Array.Empty<IDependency>()), out model);
        }
        
        public static bool New<T>(ViewLink link, ViewLink parent, out T model, params IDependency[] dependencies) where T : IDependency, new() {
            View[] views = UnityObject.Instantiate(link.link, parent.link.transform).GetComponents<View>();
            string contextKey = ProjectContext.activeContext.key;
            return TryExtractModel(views, contextKey, FillDependencies(views, dependencies), out model);
        }
        
        public static bool New<T1, T2>(ViewLink link, ViewLink parent, out (T1, T2) model) where T1 : IDependency, new() where T2 : IDependency, new() {
            View[] views = UnityObject.Instantiate(link.link, parent.link.transform).GetComponents<View>();
            string contextKey = ProjectContext.activeContext.key;
            List<IDependency> result = FillDependencies(views, Array.Empty<IDependency>());
            
            if (TryExtractModel(views, contextKey, result, out T1 first) && TryExtractModel(views, contextKey, result, out T2 second)) {
                model = (first, second);
                return true;
            }
            
            model = default;
            return false;
        }
        
        public static bool New<T1, T2>(ViewLink link, ViewLink parent, out (T1, T2) model, params IDependency[] dependencies)
            where T1 : IDependency, new() where T2 : IDependency, new() {
            View[] views = UnityObject.Instantiate(link.link, parent.link.transform).GetComponents<View>();
            string contextKey = ProjectContext.activeContext.key;
            List<IDependency> result = FillDependencies(views, dependencies);
            
            if (TryExtractModel(views, contextKey, result, out T1 first) && TryExtractModel(views, contextKey, result, out T2 second)) {
                model = (first, second);
                return true;
            }
            
            model = default;
            return false;
        }
        
        public static bool New<T>(ViewLink link, Vector3 position, Quaternion rotation, out T model) where T : IDependency, new() {
            View[] views = UnityObject.Instantiate(link.link, position, rotation).GetComponents<View>();
            string contextKey = ProjectContext.activeContext.key;
            return TryExtractModel(views, contextKey, FillDependencies(views, Array.Empty<IDependency>()), out model);
        }
        
        public static bool New<T>(ViewLink link, Vector3 position, Quaternion rotation, out T model, params IDependency[] dependencies) where T : IDependency, new() {
            View[] views = UnityObject.Instantiate(link.link, position, rotation).GetComponents<View>();
            string contextKey = ProjectContext.activeContext.key;
            return TryExtractModel(views, contextKey, FillDependencies(views, dependencies), out model);
        }
        
        public static bool New<T1, T2>(ViewLink link, Vector3 position, Quaternion rotation, out (T1, T2) model)
            where T1 : IDependency, new() where T2 : IDependency, new() {
            View[] views = UnityObject.Instantiate(link.link, position, rotation).GetComponents<View>();
            string contextKey = ProjectContext.activeContext.key;
            
            List<IDependency> result = FillDependencies(views, Array.Empty<IDependency>());
            
            if (TryExtractModel(views, contextKey, result, out T1 first) && TryExtractModel(views, contextKey, result, out T2 second)) {
                model = (first, second);
                return true;
            }
            
            model = default;
            return false;
        }
        
        public static bool New<T1, T2>(ViewLink link, Vector3 position, Quaternion rotation, out (T1, T2) model, params IDependency[] dependencies)
            where T1 : IDependency, new() where T2 : IDependency, new() {
            View[] views = UnityObject.Instantiate(link.link, position, rotation).GetComponents<View>();
            string contextKey = ProjectContext.activeContext.key;
            
            List<IDependency> result = FillDependencies(views, dependencies);
            
            if (TryExtractModel(views, contextKey, result, out T1 first) && TryExtractModel(views, contextKey, result, out T2 second)) {
                model = (first, second);
                return true;
            }
            
            model = default;
            return false;
        }
        
        public static bool New<T>(ViewLink link, Vector3 position, Quaternion rotation, ViewLink parent, out T model) where T : IDependency, new() {
            View[] views = UnityObject.Instantiate(link.link, position, rotation, parent.link.transform).GetComponents<View>();
            string contextKey = ProjectContext.activeContext.key;
            return TryExtractModel(views, contextKey, FillDependencies(views, Array.Empty<IDependency>()), out model);
        }
        
        public static bool New<T>(ViewLink link, Vector3 position, Quaternion rotation, ViewLink parent, out T model, params IDependency[] dependencies) where T : IDependency, new() {
            View[] views = UnityObject.Instantiate(link.link, position, rotation, parent.link.transform).GetComponents<View>();
            string contextKey = ProjectContext.activeContext.key;
            return TryExtractModel(views, contextKey, FillDependencies(views, dependencies), out model);
        }
        
        public static bool New<T1, T2>(ViewLink link, Vector3 position, Quaternion rotation, ViewLink parent, out (T1, T2) model)
            where T1 : IDependency, new() where T2 : IDependency, new() {
            View[] views = UnityObject.Instantiate(link.link, position, rotation, parent.link.transform).GetComponents<View>();
            string contextKey = ProjectContext.activeContext.key;
            
            List<IDependency> result = FillDependencies(views, Array.Empty<IDependency>());
            
            if (TryExtractModel(views, contextKey, result, out T1 first) && TryExtractModel(views, contextKey, result, out T2 second)) {
                model = (first, second);
                return true;
            }
            
            model = default;
            return false;
        }
        
        public static bool New<T1, T2>(ViewLink link, Vector3 position, Quaternion rotation, ViewLink parent, out (T1, T2) model, params IDependency[] dependencies)
            where T1 : IDependency, new() where T2 : IDependency, new() {
            View[] views = UnityObject.Instantiate(link.link, position, rotation, parent.link.transform).GetComponents<View>();
            string contextKey = ProjectContext.activeContext.key;
            
            List<IDependency> result = FillDependencies(views, dependencies);
            
            if (TryExtractModel(views, contextKey, result, out T1 first) && TryExtractModel(views, contextKey, result, out T2 second)) {
                model = (first, second);
                return true;
            }
            
            model = default;
            return false;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryExtractModel<T>(View[] views, string contextKey, List<IDependency> dependencies, out T model) where T : IDependency, new() {
            if (dependencies.Count > 0) {
                IDependency[] array = dependencies.ToArray();
                
                for (int viewId = 0; viewId < views.Length; viewId++) {
                    if (ProjectBinding.TryBind(out T extracted, array) && ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                        context.Connect(views[viewId], resolving => ResolveUtility.Resolve(resolving, new DependencyContainer(extracted)));
                        views[viewId].connectState = View.ConnectState.Connected;
                        model = extracted;
                        return true;
                    }
                }
            } else {
                for (int viewId = 0; viewId < views.Length; viewId++) {
                    if (ProjectBinding.TryBind(out T extracted) && ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                        context.Connect(views[viewId], resolving => ResolveUtility.Resolve(resolving, new DependencyContainer(extracted)));
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