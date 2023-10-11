using System.Collections.Generic;
using TinyDI.Dependencies.Models;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Boot {
    public abstract class BootModels {
    #if ODIN_INSPECTOR
        [ShowInInspector, HideLabel, HideInEditorMode, HideReferenceObjectPicker]
    #endif
        public ModelsContainer container { get; private set; }

        protected List<IModel> _models;

        public virtual void Create() {
            _models = new List<IModel>();
        }

        public ModelsContainer CreateContainer() {
            container = new ModelsContainer();

            for (int modelId = 0; modelId < _models.Count; modelId++) {
                container.Add(_models[modelId]);
            }

            return container;
        }
    }
}