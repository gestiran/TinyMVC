using System.Collections.Generic;
using Sirenix.OdinInspector;
using TinyDI.Dependencies.Models;
using TinyDI.Dependencies.Parameters;
using TinyDI.Resolving;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TinyMVC.Boot {
    [DisallowMultipleComponent]
    public sealed class ProjectBootstrap : MonoBehaviour {
        private sealed class ParametersContext : ContextLink<ParametersContainer> {
            public ParametersContext(Scene scene, ParametersContainer context) : base(scene, context) { }
        }
    
        private sealed class ModelsContext : ContextLink<ModelsContainer> {
            public ModelsContext(Scene scene, ModelsContainer context) : base(scene, context) { }
        }
    
        private sealed class BootstrapContext : ContextLink<IContext> {
            public BootstrapContext(Scene scene, IContext context) : base(scene, context) { }
        }
        
        [BoxGroup("Debug"), ShowInInspector, InlineProperty, HideInEditorMode, HideReferenceObjectPicker, Searchable]
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, DraggableItems = false)]
        private List<ParametersContext> _parameters;
        
        [BoxGroup("Debug"), ShowInInspector, InlineProperty, HideInEditorMode, HideReferenceObjectPicker, Searchable]
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, DraggableItems = false)]
        private List<ModelsContext> _models;
        
        [BoxGroup("Debug"), ShowInInspector, InlineProperty, HideInEditorMode, HideReferenceObjectPicker, Searchable]
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, DraggableItems = false)]
        private List<BootstrapContext> _contexts;
        
        public void Awake() {
            DontDestroyOnLoad(this);

            _parameters = new List<ParametersContext>();
            _models = new List<ModelsContext>();
            _contexts = new List<BootstrapContext>();
        }

        public void Start() {
            SceneManager.sceneLoaded += InitScene;
            SceneManager.sceneUnloaded += UnloadScene;
            
            InitScene(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        }
        
        public void AddParameters(Scene scene, ParametersContainer parameters) {
            if (TryGetContext(_parameters, scene, out ParametersContainer _, out int _)) {
                return;
            }
            
            _parameters.Add(new ParametersContext(scene, parameters));
        }

        public void AddModels(Scene scene, ModelsContainer models) {
            if (TryGetContext(_models, scene, out ModelsContainer _, out int _)) {
                return;
            }
            
            _models.Add(new ModelsContext(scene, models));
        }

        public void ResolveParameters(IParametersResolving resolving) => resolving.Resolve(CreateParametersContainers());

        public void ResolveParameters(List<IParametersResolving> resolving) => resolving.Resolve(CreateParametersContainers());

        public void ResolveModels(IModelsResolving resolving) => resolving.Resolve(CreateModelsContainers());

        public void ResolveModels(List<IModelsResolving> resolving) => resolving.Resolve(CreateModelsContainers());

        private ParametersContainer[] CreateParametersContainers() {
            ParametersContainer[] containers = new ParametersContainer[_parameters.Count];

            for (int parameterId = 0; parameterId < _parameters.Count; parameterId++) {
                containers[parameterId] = _parameters[parameterId].context;
            }

            return containers;
        }
        
        private ModelsContainer[] CreateModelsContainers() {
            ModelsContainer[] containers = new ModelsContainer[_models.Count];

            for (int modelId = 0; modelId < _models.Count; modelId++) {
                containers[modelId] = _models[modelId].context;
            }

            return containers;
        }
        
        

        private void InitScene(Scene scene, LoadSceneMode mode) {
            IContext context = GetSceneContext(scene);
            
            context.Create();
            context.Init(this, scene);
            
            _contexts.Add(new BootstrapContext(scene, context));
        }

        private IContext GetSceneContext(Scene scene) {
            GameObject[] rootObjects = scene.GetRootGameObjects();

            for (int rootId = 0; rootId < rootObjects.Length; rootId++) {
                IContext context = rootObjects[rootId].GetComponent<IContext>();

                if (context == null) {
                    continue;
                }

                return context;
            }

        #if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.LogError($"ProjectContext.GetSceneBootstrap() - {scene.name} not contain Bootstrap!");
        #endif
            
            return null;
        }
        
        private void UnloadScene(Scene scene) {
            if (!TryGetContext(_contexts, scene, out IContext context, out int contextId)) {
                return;
            }
            
            context.Unload();

            if (context is not IHaveProjectModels && TryGetContext(_models, scene, out ModelsContainer _, out int modelsId)) {
                _models.RemoveAt(modelsId);
            }

            if (context is not IHaveProjectParameters && TryGetContext(_parameters, scene, out ParametersContainer _, out int parametersId)) {
                _parameters.RemoveAt(parametersId);
            }
            
            _contexts.RemoveAt(contextId);
        }
        
        private bool TryGetContext<T1, T2>(List<T1> list, Scene target, out T2 context, out int id) where T1 : ContextLink<T2> {
            for (int contextId = 0; contextId < list.Count; contextId++) {
                if (list[contextId].scene != target) {
                    continue;
                }

                context = list[contextId].context;
                id = contextId;
                return true;
            }
            
            context = default;
            id = default;
            return false;
        }
    }
}