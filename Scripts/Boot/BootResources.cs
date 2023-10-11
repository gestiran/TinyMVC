using System.Collections.Generic;
using TinyDI.Dependencies.Parameters;

namespace TinyMVC.Boot {
    public abstract class BootResources {
        protected List<IParameters> _parameters;

        public virtual void Create() {
            _parameters = new List<IParameters>();
        }

        public ParametersContainer CreateContainer() {
            ParametersContainer container = new ParametersContainer();

            for (int parameterId = 0; parameterId < _parameters.Count; parameterId++) {
                container.Add(_parameters[parameterId]);
            }
            
            return container;
        }
    }
}