// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TinyMVC.Boot.Contexts;
using TinyMVC.Controllers;
using TinyMVC.Dependencies;
using TinyMVC.Dependencies.Extensions;
using TinyReactive;
using TinyReactive.Fields;
using TinyUtilities.Logger;

namespace TinyMVC.Boot.Extensions {
    internal static class ContextExtension {
        private const int _DEPENDENCIES_CAPACITY = 64;
        
        /// <summary> Creates all context sub-systems: controllers, models, parameters, views and modules. </summary>
        internal static void Create(this IContext context) {
            context.controllers.ConnectUnload(context.unloadPool);
            context.models.ConnectUnload(context.unloadPool);
            
            context.views.Instantiate();
            InstantiateComponents(context);
            
            context.controllers.CreateControllers();
            CreateComponentsControllers(context, context.controllers.systems, context.controllers.initLazyList);
            
            context.views.CreateViews();
            AddComponentsViews(context, context.views);
        }
        
        /// <summary> Runs the full initialization sequence: view init, resolve, begin play and loop registration. </summary>
        internal static async Task InitAsync(this IContext context) {
            await context.views.InitAsync();
            
            await Resolve(context);
            
            await context.controllers.BeginPlay();
            await context.views.BeginPlay();
        }
        
        /// <summary> Unloads the context pool, per-controller pools and resets the cancellation source. </summary>
        internal static void Unload(this IContext context) {
            if (context.unloadPool == null) {
                return;
            }
            
            try {
                context.unloadPool.Unload();
            } catch (Exception exception) {
                DebugUtility.LogError(new Exception("SceneContext.Unload with exception!", exception));
            }
            
            context.controllers.Unload();
            context.models.Unload();
            context.views.Unload();
            
            foreach (UnloadPool unload in context.unloads.Values) {
                if (unload.isUnloaded == false) {
                    unload.Unload();
                }
            }
            
            context.unloads.Clear();
        }
        
        /// <summary> Dependency resolution stage: parameters → views → binders → models → controllers. </summary>
        private static async Task Resolve(this IContext context) {
            string key = context.key;
            
            List<IDependency> dependenciesParameters = new List<IDependency>(_DEPENDENCIES_CAPACITY);
            List<IDependency> dependenciesViews = new List<IDependency>(_DEPENDENCIES_CAPACITY);
            
            ParametersContext parameters = context.parameters;
            parameters.Init();
            CreateParametersComponents(context, parameters.all);
            
            parameters.AddDependencies(dependenciesParameters);
            
            ProjectContext.data.Add(key, dependenciesParameters);
            DependencyContainer tempContainer = new DependencyContainer(dependenciesParameters);
            ProjectContext.data.tempContainer = tempContainer;
            
            context.views.GetDependencies(dependenciesViews);
            
            ProjectContext.data.viewsContainer = new DependencyContainer(dependenciesViews);
            
            ModelsContext models = context.models;
            models.CreateBinders(key);
            CreateBindersComponents(context, models);
            
            List<IDependency> runtimeDependencies = new List<IDependency>(_DEPENDENCIES_CAPACITY);
            
            runtimeDependencies.AddRange(models.dependenciesBinded);
            
            tempContainer = new DependencyContainer(runtimeDependencies);
            ProjectContext.data.tempContainer = tempContainer;
            models.TryApplyResolving();
            
            models.Create();
            CreateModelsComponents(context, models.dependencies);
            ProjectContext.data.Add(key, models.dependencies);
            
            ControllersContext controllers = context.controllers;
            controllers.Init();
            
            await controllers.InitAsync();
            
            controllers.systems.TryApplyResolving();
            context.views.TryApplyResolving();
        }
        
        private static void InstantiateComponents(IContext root) {
            foreach (IContextComponent component in root.Components()) {
                component.Instantiate();
            }
        }
        
        private static void CreateComponentsControllers(IContext root, List<IController> systems, List<ActionListener> initSystemsLazy) {
            foreach (IContextComponent component in root.Components()) {
                component.CreateControllers(systems, initSystemsLazy);
            }
        }
        
        private static void AddComponentsViews(IContext root, IViewsContext context) {
            foreach (IContextComponent component in root.Components()) {
                component.AddComponentsViews(context);
            }
        }
        
        private static void CreateParametersComponents(IContext root, List<IDependency> dependencies) {
            foreach (IContextComponent component in root.Components()) {
                component.CreateParameters(dependencies);
            }
        }
        
        private static void CreateBindersComponents<T>(IContext root, T context) where T : ModelsContext {
            foreach (IContextComponent component in root.Components()) {
                component.CreateBinders(context);
            }
        }
        
        private static void CreateModelsComponents(IContext root, List<IDependency> dependencies) {
            foreach (IContextComponent component in root.Components()) {
                component.CreateModels(dependencies);
            }
        }
    }
}