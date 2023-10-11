using System.Collections.Generic;
using Sirenix.OdinInspector;
using TinyDI.Dependencies.Models;

namespace TinyMVC.Boot {
    public abstract class BootModels {
        [ShowInInspector, HideLabel, HideInEditorMode, HideReferenceObjectPicker]
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