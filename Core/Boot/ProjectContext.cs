// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Collections.Generic;
using System.Threading.Tasks;
using TinyMVC.Boot.Extensions;

namespace TinyMVC.Boot {
    /// <summary> Global registry of contexts and their data. </summary>
    public static class ProjectContext {
        public static ProjectComponents components { get; private set; }
        public static ProjectData data { get; private set; }
        public static IContext scene { get; private set; }
        
        private static readonly Dictionary<string, IContext> _contexts;
        private static readonly Dictionary<int, List<IContext>> _sceneContexts;
        
        static ProjectContext() {
            components = new ProjectComponents();
            data = new ProjectData(components);
            
            _contexts = new Dictionary<string, IContext>();
            _sceneContexts = new Dictionary<int, List<IContext>>();
        }
        
        public static IEnumerable<IContext> Contexts() {
            foreach (IContext context in _contexts.Values) {
                yield return context;
            }
        }
        
        public static bool TryGetContext(string contextKey, out IContext context) => _contexts.TryGetValue(contextKey, out context);
        
        public static bool TryGetContext<T>(string contextKey, out T context) where T : class, IContext {
            if (_contexts.TryGetValue(contextKey, out IContext current) && current is T target) {
                context = target;
                return true;
            }
            
            context = null;
            return false;
        }
        
        internal static async Task AddContext<T>(T context, int id) where T : IContext {
            if (_contexts.TryAdd(context.key, context) == false) {
                return;
            }
            
            if (_sceneContexts.TryGetValue(id, out List<IContext> list)) {
                if (list.Contains(context) == false) {
                    list.Add(context);
                }
            } else {
                _sceneContexts.Add(id, new List<IContext>() { context });
            }
            
            scene = context;
            context.Create();
            await context.InitAsync();
        }
        
        internal static void RemoveContext<T>(T context, int sceneId) where T : class, IContext {
            if (_sceneContexts.TryGetValue(sceneId, out List<IContext> list)) {
                if (list.Contains(context)) {
                    list.Remove(context);
                }
            }
            
            context.Unload();
            _contexts.Remove(context.key);
            data.Remove(context.key);
        }
        
        internal static async Task RemoveContexts(int sceneBuildIndex) {
            if (_sceneContexts.TryGetValue(sceneBuildIndex, out List<IContext> contexts) == false) {
                return;
            }
            
            IContext[] contextsArray = contexts.ToArray();
            
            for (int contextId = 0; contextId < contextsArray.Length; contextId++) {
                await contextsArray[contextId].Remove();
            }
            
            contexts.Clear();
            
            _sceneContexts.Remove(sceneBuildIndex);
        }
        
        /// <summary> First project context creating </summary>
        internal static void Clear() {
            components.Clear();
            data.Clear();
            
            _contexts.Clear();
            _sceneContexts.Clear();
        }
    }
}